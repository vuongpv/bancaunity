using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFramework
{
	/// <summary>
	/// Update scheduler to shedule a specific job
	/// </summary>
	public class JobScheduler
	{
		/// <summary>
		/// A single job
		/// </summary>
		class Job
		{
			// Interval of update
			public int intervalMs;

			// Next update tick milestone
			public int nextRunTime;

			// The job itself
			public Func<object, bool> callback;

			// Parameters for the callback
			public object @params;
		}

		// Time precision multiplier to compare
		public float timePrecisionMultiplier { get; set; }

		// Job list
		private Dictionary<string, Job> jobList = new Dictionary<string, Job>();

		/// <summary>
		/// Get number of jobs
		/// </summary>
		public int Count { get { return jobList.Count; } }

		/// <summary>
		/// 
		/// </summary>
		public JobScheduler()
		{
			timePrecisionMultiplier = 1.0f;
		}

		/// <summary>
		/// Add a single job
		/// </summary>
		public void AddJob(string jobName, Func<object, bool> callback, int firstInMs, int regularInMs, object @params)
		{
			Job job = new Job();
			job.intervalMs = regularInMs;
			job.nextRunTime = Environment.TickCount + firstInMs;
			job.callback = callback;
			job.@params = @params;

			jobList.Add(jobName, job);
		}

		/// <summary>
		/// Remove a job
		/// </summary>
		public void RemoveJob(string jobName)
		{
			if (jobList.ContainsKey(jobName))
				jobList.Remove(jobName);
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasJob(string jobName)
		{
			return jobList.ContainsKey(jobName);
		}

		/// <summary>
		/// Update time to execute job
		/// </summary>
		/// <returns>Return next tick inverval to schedule next job</returns>
		public int Update()
		{
			List<string> removeJobs = null;

			int curTime = Environment.TickCount;
			int nearestRunTime = int.MaxValue;
			foreach (var pair in jobList)
			{
				Job job = pair.Value;
				bool isPass = false;
				if (timePrecisionMultiplier < 1)
				{
					if ((int)Math.Round(curTime * timePrecisionMultiplier) >= (int)Math.Round(job.nextRunTime * timePrecisionMultiplier))
						isPass = true;
				}
				else if( curTime >= job.nextRunTime )
				{
					isPass = true;
				}

				if (isPass)
				{
					// Job call and remove if return false
					if (!job.callback(job.@params))
					{
						if (removeJobs == null)
							removeJobs = new List<string>();
						removeJobs.Add(pair.Key);
					}
					else
					{
						// calculate nearest update time
						job.nextRunTime = curTime + job.intervalMs;
						if (job.nextRunTime < nearestRunTime)
							nearestRunTime = job.nextRunTime;
					}
				}
				else
				{
					// calculate nearest update time
					if (job.nextRunTime < nearestRunTime)
						nearestRunTime = job.nextRunTime;
				}
			}

			// Remove the jobs in remove list
			if (removeJobs != null && removeJobs.Count > 0)
			{
				foreach (var key in removeJobs)
					jobList.Remove(key);
			}

			return nearestRunTime == int.MaxValue ? int.MaxValue : nearestRunTime - curTime;
		}

		/// <summary>
		/// Force run all jobs
		/// </summary>
		public void RunAllJobsNow()
		{
			foreach (var pair in jobList)
			{
				pair.Value.callback(pair.Value.@params);
			}
		}
	}
}
