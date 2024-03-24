using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowser.Models
{
    internal class LeafNode: BrowserNode
    {
        TypeMember model;
        public string? MainInfo => model.ToString();

        public LeafNode(TypeMember model)
        {
            this.model = model;
        }
    }
}
