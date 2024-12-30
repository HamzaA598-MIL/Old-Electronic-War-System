using System;

namespace ElecWarSystem.ViewModel
{
    public class EmailUtilies
    {
        public static String chooseFileIcon(string extension)
        {
            String icon = null;
            switch (extension.ToLower())
            {
                case "pdf":
                    icon = "~/Images/pdf1.png";
                    break;
                case "dot":
                case "dotx":
                case "docm":
                case "docx":
                case "doc":
                    icon = "~/Images/word2.png";
                    break;
                case "png":
                case "jpeg":
                case "jpg":
                case "tif":
                    icon = "~/Images/image.png";
                    break;
                case "pptx":
                case "pptm":
                case "ppt":
                case "potx":
                    icon = "~/Images/powerpoint2.png";
                    break;
                case "accdb":
                case "mdb":
                    icon = "~/Images/access1.png";
                    break;
                case "xlsx":
                case "xls":
                case "xlsm":
                case "csv":
                    icon = "~/Images/excel1.png";
                    break;
                case "zip":
                case "rar":
                    icon = "~/Images/zip.png";
                    break;
                case "mp3":
                case "aac":
                case "oog":
                case "wav":
                    icon = "~/Images/mp3.ico";
                    break;
                case "mp4":
                case "mov":
                case "wmv":
                case "avi":
                case "mkv":
                    icon = "~/Images/video.png";
                    break;
                default:
                    icon = "~/Images/fax_icon_icons_com_52496.png";
                    break;
            }
            return icon;
        }
    }
}