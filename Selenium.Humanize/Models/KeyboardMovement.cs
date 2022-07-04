using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium.Humanize.Models
{
    /// <summary>
    /// https://github.com/saadejazz/humanTyper/blob/master/typer.py
    /// </summary>
    public class KeyboardMovement : BaseModel
    {
        public KeyboardMovement(ref IWebDriver driver, ref Random random) : base(ref driver, ref random) { }

        /// <summary>
        /// Types like a human (with errors)
        /// </summary>
        /// <param name="text">The text we want to write</param>
        /// <param name="from">The speed from</param>
        /// <param name="to">The speed to</param>
        public void Type(string text, int from = 150, int to = 230)
        {
            foreach (var c in text)
            {
                Thread.Sleep(Rnd.Next(from, to));

                Actions action = new Actions(Driver);
                action.SendKeys(c.ToString()).Perform();
            }
        }

    }

    //    public class Typer
    //    {
    //        char[][] qwertyKeyboardArray = {
    //        new char[] {'`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '='},
    //        new char[] {'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[', ']', '\\'},
    //        new char[] {'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\'', '\n'},
    //        new char[] {'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/'},
    //        new char[] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
    //        };

    //        char[][] qwertyShiftedKeyboardArray = {
    //        new char[] {'~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+'},
    //        new char[] {'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', '{', '}', '|'},
    //        new char[] {'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', ':', '"'},
    //        new char[] {'Z', 'X', 'C', 'V', 'B', 'N', 'M', '<', '>', '?'},
    //        new char[] {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '}
    //        };

    //        private Random Rnd;
    //        private int Distance;
    //        private TimeSpan TypingDelayMin;
    //        private TimeSpan TypingDelayMax;
    //        private float Accuracy;
    //        private float CorrectionChance;


    //        public Typer(TimeSpan typingDelayMin, TimeSpan typingDelayMax, float accuracy = 0.9f, float correctionChance = 0.6f, int distance = 1)
    //        {
    //            this.Distance = distance;
    //            this.TypingDelayMin = typingDelayMin;
    //            this.TypingDelayMax = typingDelayMax;
    //            this.Accuracy = accuracy;
    //            this.CorrectionChance = correctionChance;
    //        }

    //        public int GetDelayMs()
    //        {
    //            return Rnd.Next((int)TypingDelayMin.TotalMilliseconds, (int)TypingDelayMax.TotalMilliseconds);
    //        }

    //        public void Send(IWebElement element, string text)
    //        {
    //            char castLetter;
    //            var isError = false;
    //            var @true = "";
    //            foreach (var letter in text)
    //            {
    //                if (Rnd.NextDouble() > this.Accuracy)
    //                {
    //                    // make error
    //                    castLetter = this.WrongCharacterChoice(letter, dist: this.Distance);
    //                    isError = true;
    //                }
    //                else
    //                {
    //                    castLetter = letter;
    //                }
    //                // send a character and wait according to typing delay
    //                element.SendKeys(castLetter.ToString());
    //                Task.Delay(GetDelayMs());
    //                // record true values for errors made
    //                if (isError)
    //                {
    //                    @true += letter;
    //                }
    //                if (Rnd.NextDouble() < this.CorrectionChance && isError)
    //                {
    //                    // fix error
    //                    this.SendTextOneByOne(element, @true, false);
    //                    Task.Delay(GetDelayMs());
    //                    isError = false;
    //                    @true = "";
    //                }
    //            }
    //            // type all remaining text
    //            if (isError)
    //            {
    //                this.SendTextOneByOne(element, @true);
    //            }
    //        }

    //        public void SendTextOneByOne(IWebElement element, string text, bool sendAll = true)
    //        {
    //            for (int i = 0; i < text.Length; i++)
    //            {
    //                element.SendKeys(Keys.Backspace);
    //                Task.Delay(GetDelayMs() / 4);
    //            }
    //            if (sendAll)
    //            {
    //                foreach (var k in text)
    //                {
    //                    element.SendKeys(k.ToString());
    //                    Task.Delay(GetDelayMs());
    //                }
    //            }
    //            else
    //            {
    //                Send(element, text);
    //            }
    //        }

    //        public static double GetProbability(object key, object neighbor)
    //        {
    //            // http://accord-framework.net/docs/html/T_Accord_Statistics_Distributions_Multivariate_MultivariateNormalDistribution.htm
    //            var var = multivariate_normal(mean: key, cov: new List<object> {
    //                    new List<object> {
    //                        1,
    //                        0
    //                    },
    //                    new List<object> {
    //                        0,
    //                        1
    //                    }
    //                });
    //            return var.pdf(neighbor);
    //        }

    //        public virtual object getTuple(char letter)
    //        {
    //            var k = (from _tup_1 in this.qwertyKeyboardArray.Select((_p_1, _p_2) => Tuple.Create(_p_2, _p_1)).Chop((index, row) => (index, row))
    //                     let index = _tup_1.Item1
    //                     let row = _tup_1.Item2
    //                     where row.Contains(letter)
    //                     select (index, row.index(letter))).ToList();
    //            var arr = this.qwertyKeyboardArray;
    //            if (k.Count() == 0)
    //            {
    //                k = (from _tup_2 in this.qwertyShiftedKeyboardArray.Select((_p_3, _p_4) => Tuple.Create(_p_4, _p_3)).Chop((index, row) => (index, row))
    //                     let index = _tup_2.Item1
    //                     let row = _tup_2.Item2
    //                     where row.Contains(letter)
    //                     select (index, row.index(letter))).ToList();
    //                arr = this.qwertyShiftedKeyboardArray;
    //            }
    //            if (k.Count() == 0)
    //            {
    //                Console.WriteLine("Please provide English text only");
    //                return Tuple.Create((4, 0), arr);
    //            }
    //            return Tuple.Create(k[0], arr);
    //        }

    //        public static object GetAllNeighbors(object tup, int[][] arr, int dist)
    //        {
    //            var bounds = (from i in arr
    //                          select (i.Count() - 1)).ToList();
    //            var xs = new List<object>();
    //            var ys = new List<object>();
    //            var tups = new List<object>();
    //            var r = Enumerable.Range(0, dist + 1).ToList();
    //            r.AddRange(from k in r where !(k == 0) select (-1 * k));

    //            foreach (var i in r)
    //            {
    //                var val = tup[0] + i;
    //                if (val <= 4 && val >= 0)
    //                {
    //                    xs.append(val);
    //                }
    //                val = tup[1] + i;
    //                ys.append(val);
    //            }
    //            foreach (var k in Enumerable.Range(0, xs.Count))
    //            {
    //                tups += (from i in Enumerable.Range(0, ys.Count)
    //                         where ys[i] <= bounds[xs[k]] && ys[i] >= 0
    //                         select (xs[k], ys[i])).ToList();
    //            }
    //            return tups;
    //        }

    //        public char WrongCharacterChoice(char letter, int dist = 1)
    //        {
    //            var _tup_1 = Typer.getTuple(Typer, char);
    //            var tup = _tup_1.Item1;
    //            var arr = _tup_1.Item2;
    //            var j = (from a in Typer.GetAllNeighbors(tup, arr, dist)
    //                     where !(a == tup)
    //                     select a).ToList();
    //            var probs = (from i in j
    //                         select Typer.getProb(tup, i)).ToList();
    //            var ans = random.choices(j, probs, k: 1)[0];
    //            var result = Typer.getChar(ans, arr);
    //            if (arr == Typer.qwertyShiftedKeyboardArray)
    //            {
    //                try
    //                {
    //                    var @new = Typer.getChar(ans, Typer.qwertyKeyboardArray);
    //                    return random.choice(new List<object> {
    //                            result,
    //                            @new
    //                        });
    //                }
    //                catch
    //                {
    //                }
    //            }
    //            return result;
    //        }
    //    }
    //}
}
