using DatabaseConnectorPostgres.DAL;
using DatabaseConnectorPostgres.Exceptions;
using DbEngDatabaseConnectorPostgresine.DAL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectorPostgres.BLL
{
    public class YoutubeVideo : DbFeatureItem
	{
        private const string TABLE_NAME = "youtube_videos";

		public string Title
		{
			get
			{
				return Feature.Attributes["title"].ValueString;
			}
			set
			{
				Feature.Attributes["title"].Value = value;
			}
		}

		public int UserId
		{
			get
			{
				return Feature.Attributes["user_id"].ValueInt;
			}
			set
			{
				Feature.Attributes["user_id"].Value = value;
			}
		}

		public string URL
		{
			get
			{
				return Feature.Attributes["url"].ValueString;
			}
			set
			{
				Feature.Attributes["url"].Value = value;
			}
		}

		public string DateAdded
		{
			get
			{
				return Feature.Attributes["dateadded"].ValueString;
			}
			set
			{
				Feature.Attributes["dateadded"].Value = value;
			}
		}

		private YoutubeVideo(DbFeature feature) : base(feature)
		{
		}

		private static YoutubeVideo Get(DbFeature feature)
		{
			YoutubeVideo user = new YoutubeVideo(feature);
			bool flag = user.Feature == null;
			YoutubeVideo result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = user;
			}
			return result;
		}


		public static async Task<KeyValuePair<EnumYtVideos, IList<YoutubeVideo>>> GetAll(NpgsqlConnection connection)
		{

			DbFeatureClass dbFeatureClass = await DbFeatureClass.BuildDbFeatureClassAsync(connection, TABLE_NAME);
			List<DbFeature> features = await dbFeatureClass.GetFeatures();
			List<YoutubeVideo> list = new List<YoutubeVideo>();

			try
			{
				foreach (var entry in features)
				{
					DbFeature current = entry;
					list.Add(Get(current));
				}
			}
			catch
			{
				return new KeyValuePair<EnumYtVideos, IList<YoutubeVideo>>(EnumYtVideos.FAILED, list);
				throw new GetAllFeaturesException();
			}

			if(list.Count == 0)
            {
				return new KeyValuePair<EnumYtVideos, IList<YoutubeVideo>>(EnumYtVideos.NO_VIDEOS_FOUND, list);
			}
            else
            {
				return new KeyValuePair<EnumYtVideos, IList<YoutubeVideo>>(EnumYtVideos.SUCCESS, list);
			}
		}

		public enum EnumYtVideos
		{
			SUCCESS,
			NO_VIDEOS_FOUND,
			VIDEO_EXISTS,
			FAILED,
			VIDEO_NOT_FOUND,
			MULTIPLE_VIDEO_FOUND
		}

	}
}
