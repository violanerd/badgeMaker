using Catworx.BadgeMaker;
using SkiaSharp;
using System.Net.Http;

namespace CatWorx.BadgeMaker
{
  class Util
  {
    public static void PrintEmployees(List<Employee> employees)
    {
        for (int i = 0; i < employees.Count; i++) 
        {
            string template = "{0,-10}\t{1,-20}\t{2}";
            Console.WriteLine(String.Format(template, employees[i].GetId(), employees[i].GetFullName(), employees[i].GetPhotoUrl()));
        }
    }
    public static void MakeCSV(List<Employee> employees)
    {
        if (!Directory.Exists("data")) {
            Directory.CreateDirectory("data");
        }
        using (StreamWriter file = new("data/employees.csv")) 
        {
            file.WriteLine("ID,Name,PhotoUrl");
            for (int i = 0; i < employees.Count; i++)
            {
                string template = "{0},{1},{2}";
                file.WriteLine(String.Format(template, employees[i].GetId(), employees[i].GetFullName(), employees[i].GetPhotoUrl()));
            }
        }
    }

    async public static Task MakeBadges(List<Employee> employees) 
    {
        int BADGE_WIDTH = 669;
        int BADGE_HEIGHT = 1044;
        int PHOTO_LEFT_X = 184;
        int PHOTO_TOP_Y = 215;
        int PHOTO_RIGHT_X = 486;
        int PHOTO_BOTTOM_Y = 517;
        int COMPANY_NAME_Y = 150;
        int EMPLOYEE_NAME_Y = 600;
        int EMPLOYEE_ID_Y = 730;
        using(HttpClient client = new HttpClient())
        {
            for (int i = 0; i < employees.Count; i++)
            { 
               
                // import the data as a stream with http client
                SKImage photo = SKImage.FromEncodedData(
                    await client.GetStreamAsync(employees[i].GetPhotoUrl())
                    );
                // get the badge file    
                SKImage background = SKImage.FromEncodedData(File.OpenRead("badge.png"));
                
                // make a bitMap
                SKBitmap badge = new SKBitmap(BADGE_WIDTH, BADGE_HEIGHT);
                
                // make a canvas
                SKCanvas canvas = new SKCanvas(badge);  
                
                // use canvas, add badge as background
                canvas.DrawImage(background, new SKRect(0, 0, BADGE_WIDTH, BADGE_HEIGHT));
                // draw photo stream onto canvas
                canvas.DrawImage(photo, new SKRect(PHOTO_LEFT_X, PHOTO_TOP_Y, PHOTO_RIGHT_X, PHOTO_BOTTOM_Y));
                // set up paint to draw text on image
                SKPaint paint = new SKPaint();
                paint.TextSize = 42.0f;
                paint.IsAntialias = true;
                paint.Color = SKColors.White;
                paint.IsStroke = false;
                paint.TextAlign = SKTextAlign.Center;
                paint.Typeface = SKTypeface.FromFamilyName("Arial");

                // Company Name
                canvas.DrawText(employees[i].GetCompanyName(), BADGE_WIDTH / 2f, COMPANY_NAME_Y, paint);
                // Employee Name
                paint.Color = SKColors.Black;
                canvas.DrawText(employees[i].GetFullName(), BADGE_WIDTH / 2f, EMPLOYEE_NAME_Y, paint);
                // ID
                paint.Typeface = SKTypeface.FromFamilyName("Courier New");
                canvas.DrawText(employees[i].GetId().ToString(), BADGE_WIDTH / 2f, EMPLOYEE_ID_Y, paint);
                
                // get image from bitmap
                SKImage finalImage = SKImage.FromBitmap(badge);
                // encode to data we can see
                SKData data = finalImage.Encode();
                string filename = "data/employeeBadge" + i + ".png";
                data.SaveTo(File.OpenWrite(filename));
            }
        }
    }
  }
}