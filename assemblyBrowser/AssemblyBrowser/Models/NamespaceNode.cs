using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowser.Models
{
    internal class NamespaceNode: BrowserNode
    {
        NamespaceModel model;
        public string MainInfo => model.Name;
        public override IEnumerable<BrowserNode> Children => model.Types.Select(t => new TypeNode(t));
        public NamespaceNode(NamespaceModel model)
        {
            this.model = model;
        }
    }
}
