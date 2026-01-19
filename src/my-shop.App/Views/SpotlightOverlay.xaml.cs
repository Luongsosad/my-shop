using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using my_shop.App.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Windows.Foundation;

namespace my_shop.App.Views
{
    public sealed partial class SpotlightOverlay : Microsoft.UI.Xaml.Controls.UserControl
    {
        private int _currentStep = 0;
        private List<OnboardingStep> _steps = new List<OnboardingStep>();

        public event EventHandler? TourCompleted;
        public event EventHandler? TourSkipped;

        public SpotlightOverlay()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
            this.SizeChanged += OnSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateStepUI();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Reposition when window size changes
            if (_currentStep >= 0 && _currentStep < _steps.Count)
            {
                ShowCurrentStep();
            }
        }

        public void StartTour(List<OnboardingStep> steps)
        {
            _steps = steps;
            _currentStep = 0;
            ShowCurrentStep();
            Debug.WriteLine($"🔵 [Spotlight] Tour started with {steps.Count} steps");
        }

        private void ShowCurrentStep()
        {
            if (_currentStep < 0 || _currentStep >= _steps.Count)
                return;

            var step = _steps[_currentStep];

            // Update tooltip content
            TooltipIcon.Glyph = step.IconGlyph;
            TooltipTitle.Text = step.Title;
            TooltipDescription.Text = step.Description;

            // Update step counter
            StepCounter.Text = $"{_currentStep + 1} / {_steps.Count}";

            // Update progress dots
            UpdateProgressDots();

            // Update button visibility
            PrevButton.Visibility = _currentStep > 0 ? Visibility.Visible : Visibility.Collapsed;
            NextButton.Content = _currentStep == _steps.Count - 1 ? "Finish" : "Next";

            // Position spotlight
            if (step.TargetElement != null)
            {
                PositionSpotlight(step.TargetElement);
            }

            Debug.WriteLine($"🔵 [Spotlight] Showing step {_currentStep + 1}: {step.Title}");
        }

        private void PositionSpotlight(FrameworkElement targetElement)
        {
            try
            {
                // Get the position of target element relative to the overlay
                var transform = targetElement.TransformToVisual(this);
                var targetPosition = transform.TransformPoint(new Point(0, 0));

                var padding = 8.0;
                var spotX = targetPosition.X - padding;
                var spotY = targetPosition.Y - padding;
                var spotWidth = targetElement.ActualWidth + (padding * 2);
                var spotHeight = targetElement.ActualHeight + (padding * 2);

                // Position the spotlight border
                Canvas.SetLeft(SpotlightBorder, spotX);
                Canvas.SetTop(SpotlightBorder, spotY);
                SpotlightBorder.Width = spotWidth;
                SpotlightBorder.Height = spotHeight;

                // Create "hole" in the overlay by positioning rectangles around the spotlight
                UpdateOverlayHole(spotX, spotY, spotWidth, spotHeight);

                // Position tooltip near the spotlight
                PositionTooltip(targetPosition, targetElement.ActualWidth, targetElement.ActualHeight);

                Debug.WriteLine($"🔵 [Spotlight] Positioned at X:{spotX:F0}, Y:{spotY:F0}, W:{spotWidth:F0}, H:{spotHeight:F0}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [Spotlight] Error positioning: {ex.Message}");
            }
        }

        // THÊM METHOD MỚI
        private void UpdateOverlayHole(double x, double y, double width, double height)
        {
            // Top rectangle - from top to spotlight
            TopRect.Height = Math.Max(0, y);

            // Middle grid - same height as spotlight
            MiddleGrid.Height = height;
            MiddleGrid.Margin = new Thickness(0, y, 0, 0);

            // Left rectangle width
            MiddleGrid.ColumnDefinitions[0].Width = new GridLength(Math.Max(0, x));

            // Hole (spotlight) width
            MiddleGrid.ColumnDefinitions[1].Width = new GridLength(width);

            // Right rectangle takes remaining space
            MiddleGrid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);

            // Bottom rectangle - from spotlight bottom to window bottom
            BottomRect.Height = Math.Max(0, this.ActualHeight - (y + height));
        }

        private void PositionTooltip(Point targetPos, double targetWidth, double targetHeight)
        {
            // Calculate tooltip position
            var tooltipWidth = 450.0;
            var tooltipHeight = 400.0; // Approximate
            var spacing = 20.0;

            // Try to position to the right of the target
            var tooltipLeft = targetPos.X + targetWidth + spacing;
            var tooltipTop = targetPos.Y;

            // Check if tooltip goes off-screen to the right
            if (tooltipLeft + tooltipWidth > this.ActualWidth)
            {
                // Position to the left
                tooltipLeft = targetPos.X - tooltipWidth - spacing;
            }

            // Check if tooltip goes off-screen to the left
            if (tooltipLeft < 20)
            {
                // Position below
                tooltipLeft = targetPos.X + (targetWidth / 2) - (tooltipWidth / 2);
                tooltipTop = targetPos.Y + targetHeight + spacing;
            }

            // Check if tooltip goes off-screen below
            if (tooltipTop + tooltipHeight > this.ActualHeight)
            {
                // Position above
                tooltipTop = targetPos.Y - tooltipHeight - spacing;
            }

            // Final boundary checks
            tooltipLeft = Math.Max(20, Math.Min(tooltipLeft, this.ActualWidth - tooltipWidth - 20));
            tooltipTop = Math.Max(20, Math.Min(tooltipTop, this.ActualHeight - tooltipHeight - 20));

            TooltipBorder.Margin = new Thickness(tooltipLeft, tooltipTop, 0, 0);
        }

        private void UpdateProgressDots()
        {
            var accentColor = (Windows.UI.Color)
               Microsoft.UI.Xaml.Application.Current.Resources["SystemAccentColor"];

            var accentBrush = new SolidColorBrush(accentColor);

            var defaultBrush =
                (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources[
                    "ControlStrokeColorDefaultBrush"];


            var dots = new[] { Step1Dot, Step2Dot, Step3Dot, Step4Dot, Step5Dot, Step6Dot, Step7Dot };

            for (int i = 0; i < dots.Length; i++)
            {
                if (i < _steps.Count)
                {
                    dots[i].Visibility = Visibility.Visible;
                    dots[i].Fill = i <= _currentStep ? accentBrush : defaultBrush;
                }
                else
                {
                    dots[i].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void UpdateStepUI()
        {
            UpdateProgressDots();
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            if (_currentStep < _steps.Count - 1)
            {
                _currentStep++;
                ShowCurrentStep();
                Debug.WriteLine($"🔵 [Spotlight] Next -> Step {_currentStep + 1}");
            }
            else
            {
                // Tour completed
                Debug.WriteLine("✅ [Spotlight] Tour completed");
                TourCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnPrevClick(object sender, RoutedEventArgs e)
        {
            if (_currentStep > 0)
            {
                _currentStep--;
                ShowCurrentStep();
                Debug.WriteLine($"🔵 [Spotlight] Previous -> Step {_currentStep + 1}");
            }
        }

        private void OnSkipClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("🔵 [Spotlight] Tour skipped by user");
            TourSkipped?.Invoke(this, EventArgs.Empty);
        }
    }
}