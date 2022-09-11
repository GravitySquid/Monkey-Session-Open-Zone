//
// +----------------------------------------------------------+
// + Author: J Bannerman aka GravitySquid
// + Date: July 2022
// + Desc:
// +    Draw a zone based on the High and Low of 
// +    the session open (e.g. Frankfurt open to London open)
// +    Youtuber suggested to wait for the zone break out of 
// +    the zone and trade in the direction of the breakout.
// +----------------------------------------------------------+
//
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
            zoneHigh = double.MinValue;
            zoneLow = double.MaxValue;
            bool gotValues = false;
            for (int i = Bars.OpenTimes.GetIndexByTime(start); i < Bars.OpenTimes.GetIndexByTime(end); i++)
            {
                zoneHigh = Math.Max(zoneHigh, Bars.HighPrices[i]);
                zoneLow = Math.Min(zoneLow, Bars.LowPrices[i]);
                gotValues = true;
            }

            if (gotValues && !(double.IsNaN(zoneHigh) || double.IsNaN(zoneLow))) {
                Chart.DrawRectangle("Open session " + start.ToString(), start, zoneHigh, zoneEnd, zoneLow, Color.FromArgb(100, 0, 50, 255), 1).IsFilled = true;
            }
        }
    }
}