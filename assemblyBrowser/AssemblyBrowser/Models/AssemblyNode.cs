using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace AssemblyBrowser.Models
{
    internal class AssemblyNode : BrowserNode
    {
        AssemblyModel assembly;
        public string? MainInfo => assembly.Name;

        public override IEnumerable<BrowserNode> Children => assembly.Members.Select(m => new NamespaceNode(m));
        public AssemblyNode(AssemblyModel assembly)
        {
            this.assembly = assembly;
        }
    }
}
