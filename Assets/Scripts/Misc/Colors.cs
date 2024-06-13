using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public static class Colors
    {
        public static Color32 yellow = new Color32(221, 245, 107, 255);
        public static Color32 red = new Color32(255, 106, 106, 255);

        public static Gradient GoldGradient {
            get
            {
                var gradient = new Gradient();
                var colors = new GradientColorKey[2];
                colors[0] = new GradientColorKey(new Color32(190, 170, 0, 255), 0);
                colors[0] = new GradientColorKey(new Color32(255, 172, 95, 255), 1);

                var alphas = new GradientAlphaKey[2];
                alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
                alphas[1] = new GradientAlphaKey(0.0f, 1.0f);
                gradient.SetKeys(colors, alphas);

                return gradient;
            }
        }
        
        public static Gradient SilverGradient {
            get
            {
                var gradient = new Gradient();
                var colors = new GradientColorKey[2];
                colors[0] = new GradientColorKey(new Color32(130, 130, 130, 255), 0);
                colors[0] = new GradientColorKey(new Color32(79, 79, 79, 255), 1);

                var alphas = new GradientAlphaKey[2];
                alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
                alphas[1] = new GradientAlphaKey(0.0f, 1.0f);
                gradient.SetKeys(colors, alphas);

                return gradient;
            }
        }
        
        public static Gradient BronzeGradient {
            get
            {
                var gradient = new Gradient();
                var colors = new GradientColorKey[2];
                colors[0] = new GradientColorKey(new Color32(186, 144, 126, 255), 0);
                colors[0] = new GradientColorKey(new Color32(101, 67, 35, 255), 1);

                var alphas = new GradientAlphaKey[2];
                alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
                alphas[1] = new GradientAlphaKey(0.0f, 1.0f);
                gradient.SetKeys(colors, alphas);

                return gradient;
            }
        }
    }
}
