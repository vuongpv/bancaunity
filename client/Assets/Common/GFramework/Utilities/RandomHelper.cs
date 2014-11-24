using UnityEngine;
using System.Collections;
using System;

public class RandomHelper
{
	public static readonly System.Random rand = new System.Random();

	/// <summary>
	/// Roulette selector algorithm
	/// </summary>
	public static TType SelectRoulette<TType>(TType[] group, Func<TType, float> getFitness)
	{
		float sumFitness = 0;
		for (int i = 0; i < group.Length; i++)
			sumFitness += getFitness(group[i]);

		bool loop = true;
		int idx = -1;

		while (loop)
		{
			float slice = (float)rand.NextDouble() * sumFitness;

			float curFitness = 0.0f;

			for (int i = 0; i < group.Length; i++)
			{
				curFitness += getFitness(group[i]);
				if (curFitness >= slice )
				{
					loop = false;
					idx = i;
					break;
				}
			}
		}

		return group[idx];
	}

	/*public ShuffleBagCollection<float> GetShuffleRandom(ActorStateAnimation[] stateAnims)
	{
		Dictionary<float, int> weights = new Dictionary<float, int>();
		for (int i = 0; i < stateAnimations.Length; i++)
		{
			weights.Add(i, stateAnimations[i].weight);
		}
		ShuffleBagCollection<float> shuffleBag = RandomHelper.urand.ShuffleBag(weights);
		return shuffleBag;
	}*/
}
