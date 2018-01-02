using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Test.Object
{
    public class Entity : Disposer, ISupportInitialize
    {
        public void BeginInit()
        {
            throw new NotImplementedException();
        }

        public void EndInit()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            base.Dispose();

        }
    }
}
