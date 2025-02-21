using DocumentFormat.OpenXml.Drawing;
using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.GPTFriend.Domain;


namespace Nop.Plugin.Misc.GPTFriend.Data
{
    [NopMigration("2023/03/01 09:09:17", "Misc.DSSync base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
			if (!Schema.Table(nameof(GPTFriendChatMessage)).Exists())
				Create.TableFor<GPTFriendChatMessage>();
		}

	
		#endregion
	}
}