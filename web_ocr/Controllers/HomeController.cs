using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using web_ocr.Models;
using Tesseract;
using System.IO;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using TessWrapper;

namespace OCR_tessract.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private Document d = new Document();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DataProcess(string imagepath)
        {

            //d.text_data = extractTextFromImg(imagepath); //using tesseract older version teseract3.0
            d.text_data = useTess(imagepath); //using tesseract latest version teseract 5.0
            System.Diagnostics.Debug.WriteLine("dis :", d.text_data);
            return Content(d.text_data);


        }

        [HttpPost]
        public String GetImage(string raw_img) //IFormCollection collection
        {
            //var img = Request.Form["blob"].ToString();
            string image_path = "";

            if (ModelState.IsValid)
            {
                if (raw_img.Length == 0)
                    return "Please Recapture the images";

                else
                {

                    //var t = img.Substring(23);  // remove data:image/png;base64,
                    image_path = saveImage(raw_img);
                    d.imagepath = image_path;

                    System.Diagnostics.Debug.WriteLine("dis :", d.imagepath);
                    return image_path;
                }

            }

            return "Model not valid";
        }

        public string saveImage(string t)
        {
            byte[] bytes = Convert.FromBase64String(t);
            Image image; Bitmap returnImage;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
                returnImage = new Bitmap(Image.FromStream(ms, true, true), 320, 240);
            }
            var randomFileName = Guid.NewGuid().ToString().Substring(0, 4) + ".png";

            //var fullPath = Path.Combine(Server.MapPath(@"~\tess_images\"), randomFileName);
            var fullPath = Path.Combine(@"F:\pulz\submitted\web_ocr\web_ocr\wwwroot\tess_images\", randomFileName);
            returnImage.Save(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            System.Diagnostics.Debug.WriteLine("dis :", image.Height);

            return fullPath;
        }

        public string extractTextFromImg(string Imgpath)
        {
            //var dataPath = @"~\tessdata";
            var dataPath = @".\tessdata\";
            //Imgpath = @"F:\pulz\OCR_webapp\OCR_tessract\wwwroot\assets\vn.jpg";
            try
            {
                using (var tEngine = new TesseractEngine(dataPath, "eng", EngineMode.Default)) //creating the tesseract OCR engine with English as the language
                {
                    //System.Diagnostics.Debug.WriteLine("dis :", nicImg.Imagepath);
                    using (var img = Pix.LoadFromFile(Imgpath)) // Load of the image file from the Pix object which is a wrapper for Leptonica PIX structure
                    {
                        //var img1 = Pix.pixRemoveAlpha(img);

                        using (var page = tEngine.Process(img)) //process the specified image
                        {
                            string text = page.GetText(); //Gets the image's content as plain text.
                                                          //Console.WriteLine(text); //display the text
                                                          //Console.WriteLine(page.GetMeanConfidence()); //Get's the mean confidence that as a percentage of the recognized text.
                                                          //Console.ReadKey();
                            System.Diagnostics.Debug.WriteLine("this is console" + text);


                            DeleteImage(Imgpath);
                            return String.IsNullOrWhiteSpace(text) ? "Ocr is finished. No text found" : text;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
                return e.ToString();
            }
        }

        public string useTess(string Imgpath)
        {
            try
            {
                string result = TesseractEng.ReadImage(Imgpath);
                DeleteImage(Imgpath);
                return String.IsNullOrWhiteSpace(result) ? "Ocr is finished. No text found" : result;

            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
                return e.ToString();
            }
        }

        public void DeleteImage(string filepath)
        {
            //var filePath = Server.MapPath("~/Images/" + filename);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
