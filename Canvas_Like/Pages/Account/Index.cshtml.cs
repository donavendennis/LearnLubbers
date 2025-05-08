using System.Drawing.Imaging;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Identity;
using DataAccess;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Infrastructure.Models;
using System.IO;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace Canvas_Like.Pages.Payment
{
  public class IndexModel : PageModel
  {
    private readonly UnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IConverter _converter;

    public decimal tuitionCost { get; set; }
    public string Success { get; set; }
    public string ReceiptPath { get; set; }

    public IConfiguration Configuration => _configuration;

    public IndexModel(UnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IConfiguration configuration, IConverter converter)
    {
      _unitOfWork = unitOfWork;
      _userManager = userManager;
      _configuration = configuration;
      _converter = converter;
      tuitionCost = 0;
      ReceiptPath = String.Empty;
    }

    public void OnGet()
    {
      tuitionCost = CalculateTuitionCost();
    }

    public async Task<IActionResult> OnPostPayAsync(PaymentRequest paymentRequest)
    {

      StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

      var service = new ChargeService();
      var options = new ChargeCreateOptions
      {
        Amount = (long)(paymentRequest.Amount * 100),
        Currency = "usd",
        Source = paymentRequest.Token,
        // sends description of transaction to stripe
        Description = "Tuition payment",
      };


      Charge charge;

      try
      {

        charge = await service.CreateAsync(options);
        await LogPayment(charge, paymentRequest.Amount);
        tuitionCost = CalculateTuitionCost();
        Success = $"Payment of ${paymentRequest.Amount} was successful! Remaining balance: " + CalculateTuitionCost() + ".";
        ReceiptPath = GenterRepiept(charge, paymentRequest.Amount);
        return Page();
      }
      catch (StripeException ex)
      {
        ModelState.AddModelError(string.Empty, $"Payment failed: ${ex.Message}");
        return Page();
      }
    }

    private decimal CalculateTuitionCost()
    {
      decimal paidAmount = 0.00M;

      var studentId = _userManager.GetUserId(User);

      var payment = _unitOfWork.PaymentTransaction.GetAll()
          .Where(sr => sr.StudentId == studentId).Select(a => a.Amount)
          .ToList();

      var enrolledClassIds = _unitOfWork.StudentRegistration.GetAll()
      .Where(sr => sr.StudentId == studentId).Select(c => c.ClassId)
      .ToList();

      var enrolledClasses = _unitOfWork.Class.GetAll()
          .Where(eC => enrolledClassIds.Contains(eC.ClassId))
          .ToList();

      var totalCreditHours = enrolledClasses.Sum(c => c.CreditHours);

      foreach (var paid in payment)
      {
        paidAmount += paid;
      }

      // Calculate the tuition cost (assuming $100 per credit hour)
      return (totalCreditHours * 100) - paidAmount;
    }

    private async Task LogPayment(Charge charge, decimal amount)
    {
      var paymentTransaction = new PaymentTransaction
      {
        StudentId = _userManager.GetUserId(User),
        Amount = amount,
        TransactionDate = DateTime.UtcNow,
      };
      _unitOfWork.PaymentTransaction.Add(paymentTransaction);
      await _unitOfWork.CommitAsync();
    }

    public class PaymentRequest
    {
      public decimal Amount { get; set; }
      public string Token { get; set; }
    }

    public string GenterRepiept(Charge charge, decimal amount)
    {
        var recieptHTML = $@"
        <html>
            <body>
                <h1>Receipt</h1>
                <p>Transaction ID: {charge.Id} </p>
                <p>Amount: ${amount} </p>
                <p>Date: {DateTime.UtcNow} </p>
            </body>
        </html>";
        var pdfDoc = new HtmlToPdfDocument()
        {
            GlobalSettings =
            {
                ColorMode = DinkToPdf.ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = DinkToPdf.PaperKind.A4Plus,
            },
            Objects =
            {
                new ObjectSettings()
                {
                    PagesCount = true,
                    HtmlContent = recieptHTML,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };
        var pdf = _converter.Convert(pdfDoc);
        var fileName = $"{Guid.NewGuid()}.pdf";
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "receipts");
        var filePath = Path.Combine(directoryPath, fileName);

        // Ensure the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        System.IO.File.WriteAllBytes(filePath, pdf);
        return $"/receipts/{fileName}";
    }
  }
}

