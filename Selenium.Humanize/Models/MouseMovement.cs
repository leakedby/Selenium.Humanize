using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace Selenium.Humanize.Models
{
    /// <summary>
    /// https://github.com/Xetera/ghost-cursor
    /// </summary>
    public class MouseMovement : BaseModel
    {
        [ThreadStatic]
        public static Point lastCursorPosition = new Point(0, 0);

        [ThreadStatic]
        public static Size lastScrollOffset = new Size();

        public Size ViewPort
        {
            get
            {
                return (ViewPortLastUpdated.HasValue
                       && ViewPortLastUpdated.Value.AddSeconds(1) > DateTime.Now ? viewPort : GetViewPort());
            }
            set
            {
                viewPort = value;
            }
        }

        private Size viewPort;
        private DateTime? ViewPortLastUpdated { get; set; }


        public MouseMovement(ref IWebDriver driver, ref Random random) : base(ref driver, ref random)
        {
            // Choose random entry point
            if (lastCursorPosition.IsEmpty)
            {
                var decision = Rnd.Next(1, 4);
                int beginX = 0;
                int beginY = 0;

                switch (decision)
                {
                    // Top
                    case 1:
                        beginX = Rnd.Next(0, ViewPort.Width);
                        beginY = 0;
                        break;
                    // Left
                    case 2:
                        beginX = 0;
                        beginY = Rnd.Next(0, ViewPort.Height);
                        break;
                    // Right
                    case 3:
                        beginX = ViewPort.Width;
                        beginY = Rnd.Next(0, ViewPort.Height);
                        break;
                    // Bottom
                    case 4:
                        beginX = Rnd.Next(0, ViewPort.Width);
                        beginY = ViewPort.Height;
                        break;
                }

                lastCursorPosition = new Point(beginX, beginY);
            }
        }

        public void Navigate(IWebElement element)
        {
            ScrollToElement(element);
            Thread.Sleep(Rnd.Next(1000, 3000));
            MoveToElement(element, milliseconds: Rnd.Next(20, 90));
            Thread.Sleep(Rnd.Next(1000, 3000));
        }

        public void Navigate(Point position)
        {
            ScrollToPosition(position);
            Thread.Sleep(Rnd.Next(1000, 3000));
            MoveToPosition(position, milliseconds: Rnd.Next(20, 90));
            Thread.Sleep(Rnd.Next(1000, 3000));
        }

        /// <summary>
        /// Clicks like a human (with errors)
        /// </summary>
        /// <param name="allowClick">Allow left click</param>
        /// <param name="allowDoubleClick">Allow double click</param>
        /// <param name="allowRightClick">Allow right click</param>
        public void Click(bool allowClick = true, bool allowDoubleClick = true, bool allowRightClick = false)
        {
            if (!allowClick && !allowDoubleClick && !allowRightClick)
                throw new Exception("Please specify an allowed click type");

            Actions action = new Actions(Driver);

            if (Rnd.Next(1, 19) != 1 && allowClick)
            {
                // Normal click
                Thread.Sleep(93 / Rnd.Next(83, 201));
                action.Click().Perform();
            }
            else
            {
                var tmp_rand = Rnd.Next(1, 3);
                if (tmp_rand == 1 && allowDoubleClick)
                {
                    // Accidental double click
                    action.Click().Perform();
                    Thread.Sleep(Rnd.Next(43, 113) / 1000);
                    action.Click().Perform();
                }
                else if (tmp_rand == 2 && allowRightClick)
                {
                    // Accidental right click
                    action.ContextClick().Perform();
                }
            }
        }

        /// <summary>
        /// Move mouse like a human
        /// </summary>
        /// <param name="element"></param>
        /// <param name="speed"></param>
        /// <param name="deviation"></param>
        /// <param name="drawMouse"></param>
        public void MoveToElement(IWebElement element, int milliseconds, bool drawMouse = true)
        {
            // Adjust location with scroll offset
            var currentScrollOffset = GetScrollOffset();
            var destinationX = element.Location.X - currentScrollOffset.Width;
            var destinationY = element.Location.Y - currentScrollOffset.Height;

            // Select realistic click point on element
            float reduceBox = 0.20f;
            int elementPositionX = (int)(element.Size.Width * reduceBox + Rnd.NextFloat(0, element.Size.Width * (1 - 2 * reduceBox)));
            int elementPositionY = (int)(element.Size.Height * reduceBox + Rnd.NextFloat(0, element.Size.Height * (1 - 2 * reduceBox)));

            var destinationPosition = new Point(destinationX + elementPositionX, destinationY + elementPositionY);

            MoveToPosition(destinationPosition, milliseconds, drawMouse);
        }

        public void MoveToPosition(Point destinationPosition, int milliseconds, bool drawMouse = true)
        {
            // Draw the mouse with js
            if (drawMouse)
            {
                try
                {
                    Driver.ExecuteJavaScript(Helper.GetEmbeddedResourceFile("ShowMouse.js"));
                }
                catch (Exception ex) { }
            }

            var defaultMouse = new PointerInputDevice(PointerKind.Mouse, "default mouse");
            var actionBuilder = new ActionBuilder();
            IActionExecutor actionExecutor = GetDriverAs<IActionExecutor>(Driver);

            IEnumerable<int> FisherYatesShuffle(IList<int> collection, int elements)
            {
                for (var elementIndex = 0; elementIndex < collection.Count; elementIndex += 1)
                {
                    var randomIndex = Rnd.Next(0, elementIndex);
                    var currentValue = collection[elementIndex];
                    collection[elementIndex] = collection[randomIndex];
                    collection[randomIndex] = currentValue;
                }

                return collection.Take(elements);
            }

            // Calculate places we need to visit for it to look human
            var pathPoints = GeneratePath(lastCursorPosition, destinationPosition).ToArray();

            if (milliseconds <= pathPoints.Length)
            {
                var pointsPerMovement = pathPoints.Length / milliseconds;
                var remainingPoints = pathPoints.Length - milliseconds * pointsPerMovement;
                var distributionIndexes = FisherYatesShuffle(Enumerable.Range(0, milliseconds).ToArray(), remainingPoints).ToHashSet();
                var pointsUsed = 0;

                // Loop through all milliseconds
                for (var movementIndex = 0; movementIndex < milliseconds; movementIndex += 1)
                {
                    var movementPoints = pointsPerMovement;

                    if (distributionIndexes.Contains(movementIndex))
                    {
                        movementPoints += 1;
                    }

                    foreach (var point in pathPoints.Skip(pointsUsed).Take(movementPoints))
                    {
                        actionBuilder.AddAction(defaultMouse.CreatePointerMove(CoordinateOrigin.Viewport, point.X, point.Y, TimeSpan.FromMilliseconds(1)));
                    }

                    try
                    {
                        actionExecutor.PerformActions(actionBuilder.ToActionSequenceList());
                        actionBuilder = new ActionBuilder();
                    }
                    catch (Exception ex) { }

                    pointsUsed += movementPoints;
                }
            }
            else
            {
                var delayPerMovement = milliseconds / pathPoints.Length;
                var remainingMilliseconds = milliseconds - pathPoints.Length * delayPerMovement;
                var distributionIndexes = FisherYatesShuffle(Enumerable.Range(0, pathPoints.Length).ToArray(), remainingMilliseconds).ToHashSet();

                // Initialise the movements
                for (var movementIndex = 0; movementIndex < pathPoints.Length; movementIndex += 1)
                {
                    var movementDelay = delayPerMovement;

                    if (distributionIndexes.Contains(movementIndex))
                    {
                        movementDelay += 1;
                    }

                    foreach (var point in pathPoints.Skip(movementIndex).Take(1))
                    {
                        actionBuilder.AddAction(defaultMouse.CreatePointerMove(CoordinateOrigin.Viewport, point.X, point.Y, TimeSpan.FromMilliseconds(movementDelay)));
                    }

                    try
                    {
                        actionExecutor.PerformActions(actionBuilder.ToActionSequenceList());
                        actionBuilder = new ActionBuilder();
                    }
                    catch (Exception ex) { }
                }
            }

            // Update cursor
            lastCursorPosition = destinationPosition;
        }

        public void ScrollToElement(IWebElement element)
        {
            Driver.ExecuteJavaScript("arguments[0].scrollIntoView({behavior: 'smooth'});", element);
        }

        public void ScrollToPosition(Point position)
        {
            Driver.ExecuteJavaScript($"window.scrollTo({{left: {position.X}, top: {position.Y}, behavior: 'smooth'}})");
        }

        private IEnumerable<Point> GeneratePath(Point start, Point end)
        {
            // Generate randomised control points with a displacement of 15% to 30% between the start and end points

            var arcMultipliers = new[] { -1, 1 };
            var arcMultiplier = arcMultipliers[Rnd.Next(arcMultipliers.Length)];

            Point GenerateControlPoint()
            {
                var x = start.X + arcMultiplier * (Math.Abs(end.X - start.X) + 50) * 0.01 * Rnd.Next(15, 30);
                var y = start.Y + arcMultiplier * (Math.Abs(end.Y - start.Y) + 50) * 0.01 * Rnd.Next(15, 30);

                return Clamp(new Point((int)x, (int)y));
            }

            var anchorPoints = new[] { start, GenerateControlPoint(), GenerateControlPoint(), end };
            var binomialCoefficients = PascalRow(anchorPoints.Length - 1);
            var baseTime = Rnd.NextDouble() * 25;
            var defaultWidth = 100;
            var defaultDistance = Math.Pow(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2), 1 / 2);
            var steps = Math.Ceiling((Math.Log(Fitts((int)defaultDistance, defaultWidth) + 1, 2) + baseTime) * 3);


            yield return Clamp(start);

            for (var pointIndex = 0; pointIndex < steps; pointIndex += 1)
            {
                var tValue = pointIndex / steps;

                var x = 0d;
                var y = 0d;

                for (var anchorPointIndex = 0; anchorPointIndex < anchorPoints.Length; anchorPointIndex += 1)
                {
                    x += anchorPoints[anchorPointIndex].X * binomialCoefficients[anchorPointIndex] * Math.Pow(1 - tValue, 3 - anchorPointIndex) * Math.Pow(tValue, anchorPointIndex);
                    y += anchorPoints[anchorPointIndex].Y * binomialCoefficients[anchorPointIndex] * Math.Pow(1 - tValue, 3 - anchorPointIndex) * Math.Pow(tValue, anchorPointIndex);
                }

                yield return Clamp(new Point((int)x, (int)y));
            }

            yield return Clamp(end);
        }

        private Point Clamp(Point point)
        {
            point.X = Helper.Clamp(point.X, 0, ViewPort.Width);
            point.Y = Helper.Clamp(point.Y, 0, ViewPort.Height);

            return point;
        }

        private Size GetViewPort()
        {
            var maxWidth = (long)Driver.ExecuteJs(@"function getWidth() {
                  return Math.max(document.documentElement.clientWidth || 0, window.innerWidth || 0);
                } return getWidth();");

            var maxHeight = (long)Driver.ExecuteJs(@"function getHeight() {
                  return Math.max(document.documentElement.clientHeight || 0, window.innerHeight || 0);
                } return getHeight();");


            var jsPort = new Size((int)maxWidth, (int)maxHeight);

            ViewPortLastUpdated = DateTime.Now;
            viewPort = jsPort;

            return jsPort;
        }

        private Size GetScrollOffset()
        {
            var scrollHeight = (long)Driver.ExecuteJs(@"return (window.pageYOffset || (document.documentElement || document.body.parentNode || document.body).scrollTop);");
            var scrollWidth = (long)Driver.ExecuteJs(@"return (window.pageXOffset || (document.documentElement || document.body.parentNode || document.body).scrollLeft);");

            return new Size((int)scrollWidth, (int)scrollHeight);
        }

        private T GetDriverAs<T>(IWebDriver driver) where T : class
        {
            T driverAsType = driver as T;
            if (driverAsType == null)
            {
                IWrapsDriver wrapper = driver as IWrapsDriver;
                while (wrapper != null)
                {
                    driverAsType = wrapper.WrappedDriver as T;
                    if (driverAsType != null)
                    {
                        driver = wrapper.WrappedDriver;
                        break;
                    }

                    wrapper = wrapper.WrappedDriver as IWrapsDriver;
                }
            }

            return driverAsType;
        }

        private List<long> PascalRow(long rowNumber)
        {
            checked
            {
                long n = rowNumber;

                // Make the results as a list of long.
                List<long> results = new List<long>();
                long value = 1;
                results.Add(value);

                // Calculate the values.
                for (int k = 1; k <= n; k++)
                {
                    value = (value * (n + 1 - k)) / k;
                    results.Add(value);
                }

                return results;
            }
        }

        private int Fitts(int distance, int width)
        {
            var a = 0;
            var b = 2;
            var id = Math.Log(distance / width + 1, 2);
            return (int)(a + b * id);
        }


    }
}
