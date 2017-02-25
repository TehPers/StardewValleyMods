using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.Items {
    public abstract class ItemTemplate {

        public abstract string GetName(Dictionary<string, object> data);

        public abstract string GetDescription(Dictionary<string, object> data);

        public virtual int GetPrice(Dictionary<string, object> data) => -1;

        public virtual int GetEdibility(Dictionary<string, object> data) => -300;

        public virtual bool IsRecipe(Dictionary<string, object> data) => false;

        public abstract void GetTexture(Dictionary<string, object> data);

        public virtual void Save(Dictionary<string, object> data) { }

        public virtual void Load(Dictionary<string, object> data) { }
    }
}
