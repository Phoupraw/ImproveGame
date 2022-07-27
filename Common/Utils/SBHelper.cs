﻿namespace ImproveGame.Common.Utils
{
    public static class SBHelper
    {
        /// <summary>
        /// 不会改变原有的所有参数, 但是只能使用只有一个 PixelShader 的 Shader.
        /// </summary>
        public static void BeginEffect(this SpriteBatch sb, Effect effect, Matrix matrix)
        {
            sb.End();
            // SpriteSortMode 精灵排序模式
            // BlendState 混合状态
            // SamplerState 采样器状态
            // DepthStencilState 深度模板状态
            // RasterizerState 光栅化状态 (目前知道的的作用是裁剪 Begin-End 所有贴图)
            // Effect Shader 特效
            // Matrix 矩阵 (用于控制整体的放大缩小)
            // 现在 Begin 的参数都不变, 因为不需要做出修改!
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, effect, matrix);
        }

        /// <summary>
        /// 重新 Begin, 只是关闭了原来的 Effect (不会改变任何参数)
        /// </summary>
        public static void EndEffect(this SpriteBatch sb, Matrix matrix)
        {
            sb.End();
            sb.Begin(0, sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0],
                sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState, null, matrix);
        }
    }
}