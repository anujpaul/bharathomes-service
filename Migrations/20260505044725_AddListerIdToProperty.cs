using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bharathome_api.Migrations
{
    /// <inheritdoc />
    public partial class AddListerIdToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_UserProfiles_Id",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyAgents_Agents_AgentId",
                table: "PropertyAgents");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyAgents_Properties_PropertyId",
                table: "PropertyAgents");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyAmenities_Properties_PropertyId",
                table: "PropertyAmenities");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyImages_Properties_PropertyId",
                table: "PropertyImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Properties",
                table: "Properties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agents",
                table: "Agents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestItems",
                table: "TestItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyImages",
                table: "PropertyImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyAmenities",
                table: "PropertyAmenities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyAgents",
                table: "PropertyAgents");

            migrationBuilder.DropColumn(
                name: "PropertiesListed",
                table: "Properties");

            migrationBuilder.RenameTable(
                name: "Properties",
                newName: "properties");

            migrationBuilder.RenameTable(
                name: "Agents",
                newName: "agents");

            migrationBuilder.RenameTable(
                name: "UserProfiles",
                newName: "user_profiles");

            migrationBuilder.RenameTable(
                name: "TestItems",
                newName: "test_items");

            migrationBuilder.RenameTable(
                name: "PropertyImages",
                newName: "property_images");

            migrationBuilder.RenameTable(
                name: "PropertyAmenities",
                newName: "property_amenities");

            migrationBuilder.RenameTable(
                name: "PropertyAgents",
                newName: "property_agents");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "properties",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "properties",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Sqft",
                table: "properties",
                newName: "sqft");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "properties",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "properties",
                newName: "location");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "properties",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Beds",
                table: "properties",
                newName: "beds");

            migrationBuilder.RenameColumn(
                name: "Baths",
                table: "properties",
                newName: "baths");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "properties",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VastuOrientation",
                table: "properties",
                newName: "vastu_orientation");

            migrationBuilder.RenameColumn(
                name: "ReraRegistrationNumber",
                table: "properties",
                newName: "rera_registration_number");

            migrationBuilder.RenameColumn(
                name: "IsReraRegistered",
                table: "properties",
                newName: "is_rera_registered");

            migrationBuilder.RenameColumn(
                name: "IsFeatured",
                table: "properties",
                newName: "is_featured");

            migrationBuilder.RenameColumn(
                name: "ExpresswayProximity",
                table: "properties",
                newName: "expressway_proximity");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "properties",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Specialization",
                table: "agents",
                newName: "specialization");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "agents",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "agents",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ReraRegistrationNumber",
                table: "agents",
                newName: "rera_registration_number");

            migrationBuilder.RenameColumn(
                name: "OperatingLocation",
                table: "agents",
                newName: "operating_location");

            migrationBuilder.RenameColumn(
                name: "ListingsCount",
                table: "agents",
                newName: "listings_count");

            migrationBuilder.RenameColumn(
                name: "Provider",
                table: "user_profiles",
                newName: "provider");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "user_profiles",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "user_profiles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "user_profiles",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_profiles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "user_profiles",
                newName: "user_type");

            migrationBuilder.RenameColumn(
                name: "UserPhoto",
                table: "user_profiles",
                newName: "user_photo");

            migrationBuilder.RenameColumn(
                name: "PropertiesListed",
                table: "user_profiles",
                newName: "properties_listed");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "user_profiles",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "AccountStatus",
                table: "user_profiles",
                newName: "account_status");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "test_items",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "test_items",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "test_items",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "property_images",
                newName: "url");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "property_images",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PropertyId",
                table: "property_images",
                newName: "property_id");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "property_images",
                newName: "sort_order");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyImages_PropertyId",
                table: "property_images",
                newName: "ix_property_images_property_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "property_amenities",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "property_amenities",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PropertyId",
                table: "property_amenities",
                newName: "property_id");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyAmenities_PropertyId",
                table: "property_amenities",
                newName: "ix_property_amenities_property_id");

            migrationBuilder.RenameColumn(
                name: "AgentId",
                table: "property_agents",
                newName: "agent_id");

            migrationBuilder.RenameColumn(
                name: "PropertyId",
                table: "property_agents",
                newName: "property_id");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyAgents_AgentId",
                table: "property_agents",
                newName: "ix_property_agents_agent_id");

            migrationBuilder.AddColumn<string>(
                name: "lister_id",
                table: "properties",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_properties",
                table: "properties",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_agents",
                table: "agents",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_profiles",
                table: "user_profiles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_test_items",
                table: "test_items",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_property_images",
                table: "property_images",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_property_amenities",
                table: "property_amenities",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_property_agents",
                table: "property_agents",
                columns: new[] { "property_id", "agent_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_agents_user_profiles_id",
                table: "agents",
                column: "id",
                principalTable: "user_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_property_agents_agents_agent_id",
                table: "property_agents",
                column: "agent_id",
                principalTable: "agents",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_property_agents_properties_property_id",
                table: "property_agents",
                column: "property_id",
                principalTable: "properties",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_property_amenities_properties_property_id",
                table: "property_amenities",
                column: "property_id",
                principalTable: "properties",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_property_images_properties_property_id",
                table: "property_images",
                column: "property_id",
                principalTable: "properties",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_agents_user_profiles_id",
                table: "agents");

            migrationBuilder.DropForeignKey(
                name: "fk_property_agents_agents_agent_id",
                table: "property_agents");

            migrationBuilder.DropForeignKey(
                name: "fk_property_agents_properties_property_id",
                table: "property_agents");

            migrationBuilder.DropForeignKey(
                name: "fk_property_amenities_properties_property_id",
                table: "property_amenities");

            migrationBuilder.DropForeignKey(
                name: "fk_property_images_properties_property_id",
                table: "property_images");

            migrationBuilder.DropPrimaryKey(
                name: "pk_properties",
                table: "properties");

            migrationBuilder.DropPrimaryKey(
                name: "pk_agents",
                table: "agents");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_profiles",
                table: "user_profiles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_test_items",
                table: "test_items");

            migrationBuilder.DropPrimaryKey(
                name: "pk_property_images",
                table: "property_images");

            migrationBuilder.DropPrimaryKey(
                name: "pk_property_amenities",
                table: "property_amenities");

            migrationBuilder.DropPrimaryKey(
                name: "pk_property_agents",
                table: "property_agents");

            migrationBuilder.DropColumn(
                name: "lister_id",
                table: "properties");

            migrationBuilder.RenameTable(
                name: "properties",
                newName: "Properties");

            migrationBuilder.RenameTable(
                name: "agents",
                newName: "Agents");

            migrationBuilder.RenameTable(
                name: "user_profiles",
                newName: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "test_items",
                newName: "TestItems");

            migrationBuilder.RenameTable(
                name: "property_images",
                newName: "PropertyImages");

            migrationBuilder.RenameTable(
                name: "property_amenities",
                newName: "PropertyAmenities");

            migrationBuilder.RenameTable(
                name: "property_agents",
                newName: "PropertyAgents");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Properties",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Properties",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "sqft",
                table: "Properties",
                newName: "Sqft");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Properties",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "location",
                table: "Properties",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Properties",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "beds",
                table: "Properties",
                newName: "Beds");

            migrationBuilder.RenameColumn(
                name: "baths",
                table: "Properties",
                newName: "Baths");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Properties",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "vastu_orientation",
                table: "Properties",
                newName: "VastuOrientation");

            migrationBuilder.RenameColumn(
                name: "rera_registration_number",
                table: "Properties",
                newName: "ReraRegistrationNumber");

            migrationBuilder.RenameColumn(
                name: "is_rera_registered",
                table: "Properties",
                newName: "IsReraRegistered");

            migrationBuilder.RenameColumn(
                name: "is_featured",
                table: "Properties",
                newName: "IsFeatured");

            migrationBuilder.RenameColumn(
                name: "expressway_proximity",
                table: "Properties",
                newName: "ExpresswayProximity");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Properties",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "specialization",
                table: "Agents",
                newName: "Specialization");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "Agents",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Agents",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "rera_registration_number",
                table: "Agents",
                newName: "ReraRegistrationNumber");

            migrationBuilder.RenameColumn(
                name: "operating_location",
                table: "Agents",
                newName: "OperatingLocation");

            migrationBuilder.RenameColumn(
                name: "listings_count",
                table: "Agents",
                newName: "ListingsCount");

            migrationBuilder.RenameColumn(
                name: "provider",
                table: "UserProfiles",
                newName: "Provider");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "UserProfiles",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "UserProfiles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "UserProfiles",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserProfiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_type",
                table: "UserProfiles",
                newName: "UserType");

            migrationBuilder.RenameColumn(
                name: "user_photo",
                table: "UserProfiles",
                newName: "UserPhoto");

            migrationBuilder.RenameColumn(
                name: "properties_listed",
                table: "UserProfiles",
                newName: "PropertiesListed");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "UserProfiles",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "account_status",
                table: "UserProfiles",
                newName: "AccountStatus");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "TestItems",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TestItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "TestItems",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "url",
                table: "PropertyImages",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PropertyImages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "property_id",
                table: "PropertyImages",
                newName: "PropertyId");

            migrationBuilder.RenameColumn(
                name: "sort_order",
                table: "PropertyImages",
                newName: "Order");

            migrationBuilder.RenameIndex(
                name: "ix_property_images_property_id",
                table: "PropertyImages",
                newName: "IX_PropertyImages_PropertyId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "PropertyAmenities",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PropertyAmenities",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "property_id",
                table: "PropertyAmenities",
                newName: "PropertyId");

            migrationBuilder.RenameIndex(
                name: "ix_property_amenities_property_id",
                table: "PropertyAmenities",
                newName: "IX_PropertyAmenities_PropertyId");

            migrationBuilder.RenameColumn(
                name: "agent_id",
                table: "PropertyAgents",
                newName: "AgentId");

            migrationBuilder.RenameColumn(
                name: "property_id",
                table: "PropertyAgents",
                newName: "PropertyId");

            migrationBuilder.RenameIndex(
                name: "ix_property_agents_agent_id",
                table: "PropertyAgents",
                newName: "IX_PropertyAgents_AgentId");

            migrationBuilder.AddColumn<int>(
                name: "PropertiesListed",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Properties",
                table: "Properties",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agents",
                table: "Agents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestItems",
                table: "TestItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyImages",
                table: "PropertyImages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyAmenities",
                table: "PropertyAmenities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyAgents",
                table: "PropertyAgents",
                columns: new[] { "PropertyId", "AgentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_UserProfiles_Id",
                table: "Agents",
                column: "Id",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyAgents_Agents_AgentId",
                table: "PropertyAgents",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyAgents_Properties_PropertyId",
                table: "PropertyAgents",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyAmenities_Properties_PropertyId",
                table: "PropertyAmenities",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyImages_Properties_PropertyId",
                table: "PropertyImages",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
