using System;
using UnityEngine.SocialPlatforms;

namespace UnityEngine;

/// <summary>
///   <para>Generic access to the Social API.</para>
/// </summary>
public static class Social
{
	/// <summary>
	///   <para>This is the currently active social platform. </para>
	/// </summary>
	public static ISocialPlatform Active
	{
		get
		{
			return ActivePlatform.Instance;
		}
		set
		{
			ActivePlatform.Instance = value;
		}
	}

	/// <summary>
	///   <para>The local user (potentially not logged in).</para>
	/// </summary>
	public static ILocalUser localUser => Active.localUser;

	public static void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
	{
		Active.LoadUsers(userIDs, callback);
	}

	public static void ReportProgress(string achievementID, double progress, Action<bool> callback)
	{
		Active.ReportProgress(achievementID, progress, callback);
	}

	public static void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
	{
		Active.LoadAchievementDescriptions(callback);
	}

	public static void LoadAchievements(Action<IAchievement[]> callback)
	{
		Active.LoadAchievements(callback);
	}

	public static void ReportScore(long score, string board, Action<bool> callback)
	{
		Active.ReportScore(score, board, callback);
	}

	public static void LoadScores(string leaderboardID, Action<IScore[]> callback)
	{
		Active.LoadScores(leaderboardID, callback);
	}

	/// <summary>
	///   <para>Create an ILeaderboard instance.</para>
	/// </summary>
	public static ILeaderboard CreateLeaderboard()
	{
		return Active.CreateLeaderboard();
	}

	/// <summary>
	///   <para>Create an IAchievement instance.</para>
	/// </summary>
	public static IAchievement CreateAchievement()
	{
		return Active.CreateAchievement();
	}

	/// <summary>
	///   <para>Show a default/system view of the games achievements.</para>
	/// </summary>
	public static void ShowAchievementsUI()
	{
		Active.ShowAchievementsUI();
	}

	/// <summary>
	///   <para>Show a default/system view of the games leaderboards.</para>
	/// </summary>
	public static void ShowLeaderboardUI()
	{
		Active.ShowLeaderboardUI();
	}
}
