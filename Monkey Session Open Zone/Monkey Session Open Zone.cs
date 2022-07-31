using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MonkeySessionOpenZone : Indicator
    {
        [Parameter("Open Start Time hh:mm", DefaultValue = "16:00")]
        public string OpenStartTime { get; set; }

        [Parameter("Open End Time hh:mm", DefaultValue = "17:00")]
        public string OpenEndTime { get; set; }

        [Parameter("Zone length in hours", DefaultValue = "10")]
        public double ZoneHours { get; set; }

        //[Output("Main")]
        //public IndicatorDataSeries Result { get; set; }

        private DateTime openStart, openEnd;
        private double zoneHigh, zoneLow;

        protected override void Initialize()
        {
            openStart = DateTime.Parse(OpenStartTime);
            openEnd = DateTime.Parse(OpenEndTime);
            
            Print("Open start time {0}, Open end time {1}", openStart.TimeOfDay.ToString(), openEnd.TimeOfDay.ToString());
            Print("Last Bar time {0} UTC {1} local {2} ", Bars.LastBar.OpenTime, Bars.LastBar.OpenTime.ToUniversalTime(), Bars.LastBar.OpenTime.ToLocalTime());
        }

        public override void Calculate(int index)
        {
            if (Chart.TimeFrame > TimeFrame.Hour) return;
            DateTime currentDate = Bars.OpenTimes[index].ToLocalTime().Date;
            DateTime start = currentDate.AddHours(openStart.Hour).AddMinutes(openStart.Minute).ToUniversalTime();
            DateTime end = currentDate.AddHours(openEnd.Hour).AddMinutes(openEnd.Minute).ToUniversalTime();
            DateTime zoneEnd = start.AddHours(ZoneHours);
            //DateTime dtCurrentBar = Bars.OpenTimes[index].ToLocalTime();
            //DateTime dtNextBar = Bars.OpenTimes[index +1].ToLocalTime();
            zoneHigh = double.MinValue;
            zoneLow = double.MaxValue;
            bool done = false;
            for (int i = Bars.OpenTimes.GetIndexByTime(start); i < Bars.OpenTimes.GetIndexByTime(end); i++)
            {
                zoneHigh = Math.Max(zoneHigh, Bars.HighPrices[i]);
                zoneLow = Math.Min(zoneLow, Bars.LowPrices[i]);
                done = true;
            }

            Print("start index {0} end Index {1}", Bars.OpenTimes.GetIndexByTime(start), Bars.OpenTimes.GetIndexByTime(end));
            Print("done {0}", done);
            if (done) {
                //Chart.DrawRectangle("Open session " + start.ToString(), start, zoneHigh, end, zoneLow, Color.FromArgb(50, 0, 50, 255)).IsFilled = true;
                Chart.DrawRectangle("Open session " + start.ToString(), start, zoneHigh, zoneEnd, zoneLow, Color.FromArgb(100, 0, 50, 255), 1).IsFilled = true;
                //Chart.DrawTrendLine("Session Open High " + start.ToString(), start, zoneHigh, zoneEnd, zoneHigh, Color.Blue);
                //Chart.DrawTrendLine("Session Open Low " + start.ToString(), start, zoneLow, zoneEnd, zoneLow, Color.Blue);
            }
        }
    }
}