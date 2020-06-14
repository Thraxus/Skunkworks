using System.Collections.Generic;
using SkunkWorks.Thraxus.GridControl.DataTypes.Enums;

namespace SkunkWorks.Thraxus.GridControl.Models
{
	public class PidController
	{
		// https://en.wikipedia.org/wiki/PID_controller
		// https://www.habrador.com/tutorials/pid-controller/1-car-follow-path/
		// https://www.habrador.com/tutorials/pid-controller/3-stabilize-quadcopter/
		// https://www.habrador.com/tutorials/math/3-turn-left-or-right/
		// https://blog.habrador.com/2015/11/explaining-hybrid-star-pathfinding.html

		public double Kp = 1; // Proportional Gain
		public double Ki = 0; // Integral Gain
		public double Kd = 0.01; // Derivative Gain

		private long _stepLength = 5; // Ticks between recalculations
		private long _totalSteps; // How many times has this run

		private readonly Queue<double> _pastErrorQueue = new Queue<double>(3);
		
		private const double GyroClampMax = 1;
		private const double GyroClampMin = 0;

		private const double ThrusterClampMax = float.MaxValue;
		private const double ThrusterClampMin = float.MinValue;

		private readonly PidType _type;

		public PidController(PidType type)
		{
			_type = type;
		}

		public void Reset()
		{
			_stepLength = 0;
			_totalSteps = 0;
			_pastErrorQueue.Clear();
		}

		public void SetStepLength(long length)
		{
			_stepLength = length;
		}

		public void SetPidKs(double kp, double ki, double kd)
		{
			Kp = kp;
			Ki = ki;
			Kd = kd;
		}

		private double Output(double error)
		{
			double output = 0;
			double pastErrors = AverageSumPastErrors();
			
			//P
			output += Kp * error;

			//I
			output += Ki * Clamp(error * _stepLength);

			//D
			output += (error - pastErrors) / _stepLength;

			_pastErrorQueue.Enqueue(error);
			_totalSteps++;
			return output;
		}
		
		private double AverageSumPastErrors()
		{
			if (_pastErrorQueue.Count == 0) return 0;
			double sum = 0;
			for (int i = 0; i < _pastErrorQueue.Count -1; i++)
			{
				double num = _pastErrorQueue.Dequeue();
				sum += num * _stepLength;
				_pastErrorQueue.Enqueue(num);
			}
			return sum / _pastErrorQueue.Count;
		}
		
		private double Clamp(double value)
		{
			if (_type == PidType.Gyro) return value > GyroClampMax ? GyroClampMax : value < GyroClampMin ? GyroClampMin : value;
			return value > ThrusterClampMax ? ThrusterClampMax : value < ThrusterClampMin ? ThrusterClampMin : value;
		}
	}
}