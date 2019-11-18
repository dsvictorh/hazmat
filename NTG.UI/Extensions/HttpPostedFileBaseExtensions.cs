using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

public static class HttpPostedFileBaseExtensions
{
    public const int ImageMinimumBytes = 512;

    public static string IsImage(this HttpPostedFileBase postedFile)
    {
        //-------------------------------------------
        //  Check the image mime types
        //-------------------------------------------
        if (postedFile.ContentType.ToLower() != "image/jpg" &&
                    postedFile.ContentType.ToLower() != "image/jpeg" &&
                    postedFile.ContentType.ToLower() != "image/pjpeg" &&
                    postedFile.ContentType.ToLower() != "image/gif" &&
                    postedFile.ContentType.ToLower() != "image/x-png" &&
                    postedFile.ContentType.ToLower() != "image/png" &&
                    postedFile.ContentType.ToLower() != "image/x-icon")
        {
            return "Wrong Mime Type";
        }

        //-------------------------------------------
        //  Check the image extension
        //-------------------------------------------
        if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg"
            && Path.GetExtension(postedFile.FileName).ToLower() != ".ico")
        {
            return "Wrong Extension";
        }

        //-------------------------------------------
        //  Attempt to read the file and check the first bytes
        //-------------------------------------------
        try
        {
            if (!postedFile.InputStream.CanRead)
            {
                return "Input Stream Can't Read";
            }

            if (postedFile.ContentLength < ImageMinimumBytes)
            {
                return "Less Than Min Bytes";
            }

            byte[] buffer = new byte[512];
            postedFile.InputStream.Read(buffer, 0, 512);
            string content = System.Text.Encoding.UTF8.GetString(buffer);
            if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
            {
                return "Stream Reads Html";
            }
        }
        catch (Exception)
        {
            return "Failed Input Stream";
        }

        //-------------------------------------------
        //  Try to instantiate new Bitmap, if .NET will throw exception
        //  we can assume that it's not a valid image
        //-------------------------------------------

        try
        {
            //Ignore on icon files since azure will throw an exception when trying to convert the image to Bitmap or Icon
            if (postedFile.ContentType.ToLower() != "image/x-icon")
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.InputStream))
                {
                }
            }
        }
        catch (Exception)
        {
            return "Faile Bitmap";
        }

        return string.Empty;
    }
}