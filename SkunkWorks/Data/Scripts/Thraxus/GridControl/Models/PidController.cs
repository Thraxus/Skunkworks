using System;
using SkunkWorks.Thraxus.GridControl.DataTypes.Enums;
using SpaceEngineers.Game.EntityComponents.DebugRenders;
using VRageMath;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class PidController
	{   
		// https://en.wikipedia.org/wiki/PID_controller
		
		public double Kp; // Proportional Gain
		public double Ki; // Integral Gain
		public double Kd; // Derivative Gain

		public double POut;
		public double IOut;
		public double DOut;

		public double Time; // This is the interval of the examination - the "steps"
		public double Error; // This is what starts high and approaches 0
		private double _lastError; // This is the last result
		private double _lastError2; // Adding another sample 

		private long _lastTick; 

		private const double GyroClampMax = 1;
		private const double GyroClampMin = 0;

		private const double ThrusterClampMax = float.MaxValue;
		private const double ThrusterClampMin = float.MinValue;

		private PidType _type;

		public float GetCorrection(double error, long tick)
		{
			return 0;
		}

		private double CalculateCorrection(double error, long tick)
		{
			double correction = 0;




			return correction;
		}

		private double Clamp(double value)
		{
			if (_type == PidType.Gyro) return value > GyroClampMax ? GyroClampMax : value < GyroClampMin ? GyroClampMin : value;
			return value > ThrusterClampMax ? ThrusterClampMax : value < ThrusterClampMin ? ThrusterClampMin : value;
		}
	}
}