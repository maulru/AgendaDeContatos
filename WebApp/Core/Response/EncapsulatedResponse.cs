﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Response
{
    public class EncapsulatedResponse<T>
    {
        public T Value { get; set; }
    }
}
