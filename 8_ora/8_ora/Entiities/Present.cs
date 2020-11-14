using _8_ora.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8_ora.Entiities
{
    public class Present : Toy
    {
        public Present(Color ribbon, Color box)
        {

        }

        protected override void DrawImage(Graphics g)
        {
            throw new NotImplementedException();
        }
    }
}
