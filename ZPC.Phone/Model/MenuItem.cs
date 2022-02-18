using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPC.Phone.Model
{
    public class MenuItem:IDisposable
    {
        public string Name { get; set; }

        public void Dispose()
        {
           
        }

        protected void Dispose(bool disposing)
        {
            
        }

        ~MenuItem()
        {
            Dispose(false);
        }
    }
}
