using Microsoft.UI.Xaml;

namespace my_shop.App.Models
{
    public class OnboardingStep
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string IconGlyph { get; set; } = "\uE80F";
        public FrameworkElement? TargetElement { get; set; }
    }
}