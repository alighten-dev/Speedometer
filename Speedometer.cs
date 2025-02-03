//
// Copyright (C) 2024, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	public class Speedometer : Indicator
	{
		private double secondsDifference;
		private DateTime startTime;
		private DateTime endTime;
		private double barsPerTwoMinutes;
		private double speed;
		private NinjaTrader.Gui.Tools.SimpleFont speedometerFont;
		private NinjaTrader.Gui.Tools.SimpleFont speedometerLabelFont;
		private int alertDelayCounter;
		private int alertDelay;
		private Series<double> speedSeries; 

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Calculate					= Calculate.OnBarClose;
				Description					= @"Speedometer";
				Name						= "Speedometer";
				DrawOnPricePanel			= true;
				IsOverlay					= true;
				IsSuspendedWhileInactive	= true;
				
				Period						= 20;
				MinThreshold				= 30;
				BasisTextLocation 			= TextPosition.TopRight;
				SlowColor					= Brushes.Black;
				FastColor					= Brushes.Green;
				ReceiveTextAlerts			= false;
				TextAlertDelayBars			= 20;

				alertDelayCounter			= 0;

			}
			else if (State == State.Historical)
			{
				
			}
			else if (State == State.DataLoaded)
			{
				ClearOutputWindow();
				speedometerLabelFont = new NinjaTrader.Gui.Tools.SimpleFont("Arial", 12) { Size = 20, Bold = true };
				speedometerFont = new NinjaTrader.Gui.Tools.SimpleFont("Arial", 12) { Size = 50, Bold = true };
				
				speedSeries = new Series<double>(this);
				
			}
		}
		
		protected override void OnBarUpdate()
		{
			if (CurrentBar < Period+1)
                return;
			
			endTime = Time[0];
			startTime = Time[Period];
			
			secondsDifference = (endTime - startTime).TotalSeconds;
			
			barsPerTwoMinutes = Period / (secondsDifference / 60) * 2;
			
			speed = Math.Round(barsPerTwoMinutes, 0);
			
			speedSeries[0] = speed;
			
			
			Print(" ");
			Print("--------------");
			Print("Time: " + Time[0]);
			Print("speed " + speed);
			
			TextFixed speedometerLabel;
			TextFixed speedometer;
			
			if (speed >= MinThreshold)
			{				
				speedometer = Draw.TextFixed (this, "speedomenter", " fast \b" 
					+ "\n"
					+ "\n" + speed.ToString() + "", 
					BasisTextLocation, Brushes.White, speedometerFont, FastColor, FastColor, 100, DashStyleHelper.Solid, 2, false, "");				
			}
			else
			{
				speedometer = Draw.TextFixed (this, "speedomenter", " slow \b" 
					+ "\n"
					+ "\n" + speed.ToString() + "", BasisTextLocation, Brushes.White, speedometerFont, SlowColor, SlowColor, 100, DashStyleHelper.Solid, 2, false, "");
			}
			speedometer.Alignment = TextAlignment.Center;
			
			
			alertDelayCounter = alertDelayCounter + 1;
			if (speedSeries[1] < MinThreshold && speedSeries[0] >= MinThreshold && ReceiveTextAlerts && alertDelayCounter > TextAlertDelayBars)
			{
                string subject = "ALERT: SPEED IS ABOVE " + MinThreshold;
                string message = "The speed has crossed above " + MinThreshold + ".";
	            try
				{
	            	SendMail(TextMessageEmailAddress, subject, message);
				}
				catch (Exception e)
				{
					// In case the indicator has already been Terminated, you can safely ignore errors
					if (State >= State.Terminated)
						return;
					
					Log("FibRetracementTargets", NinjaTrader.Cbi.LogLevel.Warning);
					
					Print(Time[0] + " " + e.ToString());
				}
				alertDelayCounter = 0;
			}
		}

		#region Properties
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Period", Order=1, GroupName="Parameters")]
		public int Period
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MinThreshold", Order=2, GroupName="Parameters")]
		public int MinThreshold
		{ get; set; }
		
		[Display(Name="Chart location for speedometer", Order=3, GroupName="Parameters")]
		public TextPosition BasisTextLocation
		{ get; set; }
		
		[NinjaScriptProperty]
		[XmlIgnore()]
		[Display(Name = "Slow Color",  Order=4, GroupName="Parameters")]
		public Brush SlowColor
		{ get; set; }
		
		[NinjaScriptProperty]
		[XmlIgnore()]
		[Display(Name = "Fast Color",  Order=5, GroupName="Parameters")]
		public Brush FastColor
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="ReceiveTextAlerts", Order=1, GroupName="Text Alerts")]
		public bool ReceiveTextAlerts
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TextAlertDelayBars", Order=2, GroupName="Text Alerts")]
		public int TextAlertDelayBars
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Text via email address (4049789316@msg.fi.google.com)", Order=3, GroupName="Text Alerts")]
		public string TextMessageEmailAddress
		{ get; set; } = "";
		
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private Speedometer[] cacheSpeedometer;
		public Speedometer Speedometer(int period, int minThreshold, Brush slowColor, Brush fastColor, bool receiveTextAlerts, int textAlertDelayBars, string textMessageEmailAddress)
		{
			return Speedometer(Input, period, minThreshold, slowColor, fastColor, receiveTextAlerts, textAlertDelayBars, textMessageEmailAddress);
		}

		public Speedometer Speedometer(ISeries<double> input, int period, int minThreshold, Brush slowColor, Brush fastColor, bool receiveTextAlerts, int textAlertDelayBars, string textMessageEmailAddress)
		{
			if (cacheSpeedometer != null)
				for (int idx = 0; idx < cacheSpeedometer.Length; idx++)
					if (cacheSpeedometer[idx] != null && cacheSpeedometer[idx].Period == period && cacheSpeedometer[idx].MinThreshold == minThreshold && cacheSpeedometer[idx].SlowColor == slowColor && cacheSpeedometer[idx].FastColor == fastColor && cacheSpeedometer[idx].ReceiveTextAlerts == receiveTextAlerts && cacheSpeedometer[idx].TextAlertDelayBars == textAlertDelayBars && cacheSpeedometer[idx].TextMessageEmailAddress == textMessageEmailAddress && cacheSpeedometer[idx].EqualsInput(input))
						return cacheSpeedometer[idx];
			return CacheIndicator<Speedometer>(new Speedometer(){ Period = period, MinThreshold = minThreshold, SlowColor = slowColor, FastColor = fastColor, ReceiveTextAlerts = receiveTextAlerts, TextAlertDelayBars = textAlertDelayBars, TextMessageEmailAddress = textMessageEmailAddress }, input, ref cacheSpeedometer);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Speedometer Speedometer(int period, int minThreshold, Brush slowColor, Brush fastColor, bool receiveTextAlerts, int textAlertDelayBars, string textMessageEmailAddress)
		{
			return indicator.Speedometer(Input, period, minThreshold, slowColor, fastColor, receiveTextAlerts, textAlertDelayBars, textMessageEmailAddress);
		}

		public Indicators.Speedometer Speedometer(ISeries<double> input , int period, int minThreshold, Brush slowColor, Brush fastColor, bool receiveTextAlerts, int textAlertDelayBars, string textMessageEmailAddress)
		{
			return indicator.Speedometer(input, period, minThreshold, slowColor, fastColor, receiveTextAlerts, textAlertDelayBars, textMessageEmailAddress);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Speedometer Speedometer(int period, int minThreshold, Brush slowColor, Brush fastColor, bool receiveTextAlerts, int textAlertDelayBars, string textMessageEmailAddress)
		{
			return indicator.Speedometer(Input, period, minThreshold, slowColor, fastColor, receiveTextAlerts, textAlertDelayBars, textMessageEmailAddress);
		}

		public Indicators.Speedometer Speedometer(ISeries<double> input , int period, int minThreshold, Brush slowColor, Brush fastColor, bool receiveTextAlerts, int textAlertDelayBars, string textMessageEmailAddress)
		{
			return indicator.Speedometer(input, period, minThreshold, slowColor, fastColor, receiveTextAlerts, textAlertDelayBars, textMessageEmailAddress);
		}
	}
}

#endregion
