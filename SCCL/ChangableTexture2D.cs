using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TehPers.Stardew.Framework;

namespace TehPers.Stardew.SCCL {
    public class ChangableTexture2D : Texture2D {

        public ChangableTexture2D(GraphicsDevice graphicsDevice, int width, int height) : base(graphicsDevice, width, height) {

        }

        public void Update<T>(Texture2D newTex) where T : struct {
            T[] data = new T[newTex.Width * newTex.Height];
            newTex.GetData(data);
            this.Update(newTex.Width, newTex.Height, data);
        }

        public void Update<T>(int width, int height, T[] data) where T : struct {
            Texture2D tmp = new Texture2D(this.GraphicsDevice, width, height);
            tmp.SetData(data);
            tmp.CopyAllFields(this);
            tmp.Dispose();
        }
    }
}
