using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowser.Models
{
    internal class TypeNode: BrowserNode
    {
        TypeModel model;
        public string? MainInfo => model.Name;

        public override IEnumerable<BrowserNode>? Children => model.Members.Select((TypeMember m) => new LeafNode(m));
        public TypeNode(TypeModel model)
        {
            this.model = model;
        }
    }
}
