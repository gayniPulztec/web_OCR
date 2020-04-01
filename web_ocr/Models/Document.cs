using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace web_ocr.Models
{
    public class Document
    {
        public int docId { get; set; }

        [BindProperty]
        [DefaultValue("")]
        public string imagepath { get; set; }

        public string text_data { get; set; }
    }
}
