﻿using nCode.CMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace nCode.CMS.UI
{
    public class ContentPartHeaderControl : UserControl, IContentPartHeaderControl
    {
        public ContentPart ContentPart { get; set; }
    }
}