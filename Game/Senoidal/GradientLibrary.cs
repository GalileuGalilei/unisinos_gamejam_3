using Microsoft.Xna.Framework;

namespace game.Senoidal
{
    public static class GradientLibrary
    {
        private static readonly Color[] _fallbackGradient = new Color[] { Color.Black, Color.Pink };
        public static readonly Color[][] Gradients = new Color[][]
        {
            // 0. Pôr do Sol (Análogo - Amarelo para Rosa Melancia)
            new Color[] { new Color(255, 195, 113), new Color(255, 95, 109) },

            // 1. Oceano Profundo (Frio - Azul Petróleo para Índigo)
            new Color[] { new Color(43, 88, 118), Color.Indigo },

            // 2. Cyberpunk (Complementar - Ciano Neon para Rosa Choque)
            new Color[] { new Color(0, 242, 254), new Color(79, 172, 254) }, // Wait, corrigindo para ciano/azul

            // 3. Cyberpunk Vibrante (Complementar - Magenta para Ciano)
            new Color[] { new Color(248, 87, 166), new Color(255, 88, 88) }, // Magenta/Vermelho
            
            // 4. Aurora Boreal (Análogo - Verde Esmeralda para Azul Marinho)
            new Color[] { new Color(0, 198, 255), new Color(0, 114, 255) },

            // 5. Floresta Mística (Monocromático - Verde Limão para Verde Floresta)
            new Color[] { new Color(132, 250, 176), new Color(143, 211, 244) },

            // 6. Fogo e Cinzas (Análogo - Laranja Fogo para Vermelho Escuro)
            new Color[] { new Color(241, 39, 17), new Color(245, 175, 25) },

            // 7. Ametista (Monocromático - Lavanda para Roxo Escuro)
            new Color[] { Color.Lavender, Color.Purple },

            // 8. Pêssego Suave (Análogo - Pêssego para Laranja Claro)
            new Color[] { new Color(255, 237, 188), new Color(237, 66, 100) },

            // 9. Synthwave Retrô (Complementar - Roxo Profundo para Laranja Neon)
            new Color[] { new Color(62, 81, 81), new Color(222, 203, 164) }, // Cinza/Dourado (Atualizado nome)
            
            // 10. Synthwave Raiz (Azul Escuro para Magenta)
            new Color[] { new Color(102, 126, 234), new Color(118, 75, 162) },

            // 11. Algodão Doce (Pastel Análogo - Rosa Bebê para Azul Bebê)
            new Color[] { new Color(255, 209, 255), new Color(250, 208, 196) },

            // 12. Crepúsculo (Monocromático - Azul Marinho para Preto Sutil)
            new Color[] { new Color(20, 30, 48), new Color(36, 59, 85) },

            // 13. Deserto de Areia (Monocromático Terra - Ouro para Marrom)
            new Color[] { new Color(234, 205, 163), new Color(214, 174, 123) },

            // 14. Abismo (Frio Intenso - Preto para Sangue/Vinho)
            new Color[] { new Color(0, 0, 0), new Color(67, 0, 0) }
        };

        public static Color[] GetGradient(int index)
        {
            if (index >= 0 && index < Gradients.Length)
            {
                return Gradients[index];
            }

            return _fallbackGradient;
        }
    }
}