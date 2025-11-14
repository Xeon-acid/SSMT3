using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WinUI3Helper
{
    public static class VisualHelper
    {
        // ✅ 旧版本：保持兼容 Visual 参数
        public static void CreateFadeAnimation(Visual imageVisual)
        {
            var fadeAnimation = imageVisual.Compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(0.0f, 0.0f);
            fadeAnimation.InsertKeyFrame(1.0f, 1.0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);
            fadeAnimation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

            imageVisual.StartAnimation("Opacity", fadeAnimation);
        }

        public static void CreateScaleAnimation(Visual imageVisual)
        {
            var scaleAnimation = imageVisual.Compositor.CreateVector3KeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(0.0f, new Vector3(1.05f, 1.05f, 1.05f));
            scaleAnimation.InsertKeyFrame(1.0f, new Vector3(1.0f, 1.0f, 1.0f));
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(500);
            scaleAnimation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

            imageVisual.StartAnimation("Scale", scaleAnimation);
        }

        // ✅ 新增版本：支持 UIElement 参数（自动取 Visual）
        public static void CreateFadeAnimation(UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            CreateFadeAnimation(visual); // 复用旧逻辑
        }

        public static void CreateScaleAnimation(UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            CreateScaleAnimation(visual);
        }


        public static void CreateInfoBarShowAnimation(Visual visual)
        {
            var compositor = visual.Compositor;
            var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();

            // 关键帧设置：
            // 0% - 从很小的点开始
            // 40% - 快速放大到略大于正常尺寸（制造冲击感）
            // 70% - 回弹到略小于正常尺寸
            // 100% - 恢复到正常尺寸
            scaleAnimation.InsertKeyFrame(0.0f, new Vector3(0.1f, 0.1f, 1.0f));
            scaleAnimation.InsertKeyFrame(0.4f, new Vector3(1.15f, 1.15f, 1.0f));
            scaleAnimation.InsertKeyFrame(0.7f, new Vector3(0.95f, 0.95f, 1.0f));
            scaleAnimation.InsertKeyFrame(1.0f, new Vector3(1.0f, 1.0f, 1.0f));

            scaleAnimation.Duration = TimeSpan.FromMilliseconds(600);
            scaleAnimation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

            // 同时添加淡入效果
            var opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
            opacityAnimation.InsertKeyFrame(0.0f, 0.0f);
            opacityAnimation.InsertKeyFrame(0.3f, 1.0f); // 快速淡入
            opacityAnimation.Duration = TimeSpan.FromMilliseconds(300);
            opacityAnimation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

            // 启动动画
            visual.StartAnimation("Scale", scaleAnimation);
            visual.StartAnimation("Opacity", opacityAnimation);
        }

        public static void CreateInfoBarShowAnimation(UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            CreateInfoBarShowAnimation(visual);
        }

    }
}
