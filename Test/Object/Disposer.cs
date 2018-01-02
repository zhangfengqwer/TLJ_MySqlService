using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public abstract class Disposer : Object, IDisposable
    {

        
        public virtual void Dispose()
        {
            
        }
    }
}
