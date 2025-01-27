
namespace ChemSW.Nbt
{
    public enum CswEnumNbtConfigurationVariables
    {
        unknown,

        /// <summary>
        /// 1 = auditing is on; 0 = auditing is off
        /// </summary>
        auditing,

        /// <summary>
        /// Image to display on every page
        /// </summary>
        brand_pageicon,

        /// <summary>
        /// If an operation affects this number of nodes, run as a batch operation instead
        /// </summary>
        batchthreshold,

        /// <summary>
        /// Title to display on every page
        /// </summary>
        brand_pagetitle,

        /// <summary>
        /// Records the last time Nbt Meta Data was altered
        /// </summary>
        cache_lastupdated,

        /// <summary>
        /// If set to 1, users can define their own barcodes on new containers.
        /// </summary>
        custom_barcodes,

        /// <summary>
        /// Format of database (oracle, mysql, mssql)
        /// </summary>
        dbformat,

        /// <summary>
        /// If 1, display warning messages in the web interface.
        /// </summary>
        displaywarningsinui,

        /// <summary>
        /// If 1, display error messages in the web interface.
        /// </summary>
        displayerrorsinui,

        /// <summary>
        /// Number of failed login attempts before a user's account is locked.
        /// </summary>
        failedloginlimit,

        /// <summary>
        /// Number of Generators to process in each scheduler cycle
        /// </summary>
        generatorlimit,

        /// <summary>
        /// Number of Targets to generate from a Generator in each scheduler cycle
        /// </summary>
        generatortargetlimit,

        /// <summary>
        /// If 1, Schema is in Demo mode
        /// </summary>
        is_demo,

        /// <summary>
        /// Enforce license agreement on all users
        /// </summary>
        license_type,

        /// <summary>
        /// Whether to prevent editing answers once an inspection is Action Required
        /// </summary>
        lock_inspection_answer,

        /// <summary>
        /// Maximum depth of location controls
        /// </summary>
        loc_max_depth,

        /// <summary>
        /// If 1, use image-based location controls
        /// </summary>
        loc_use_images,

        /// <summary>
        /// When set to 1, total quantity to deduct in DispenseContainer cannot exceed container netquantity.
        /// </summary>
        netquantity_enforced,

        /// <summary>
        /// Number of days before a password expires
        /// </summary>
        passwordexpiry_days,

        /// <summary>
        /// User password complexity level (0 - none; 1 - letters, numbers; 2 - letters, numbers, and symbols)
        /// </summary>
        password_complexity,

        /// <summary>
        /// User password minimum length (between 0 and 20)
        /// </summary>
        password_length,

        /// <summary>
        /// Unique identifier for the schema structure
        /// </summary>
        schemaid,

        /// <summary>
        /// Version of this Schema
        /// </summary>
        schemaversion,

        /// <summary>
        /// Show the Loading box on postback
        /// </summary>
        showloadbox,

        /// <summary>
        /// Maximum number of results per tree level
        /// </summary>
        treeview_resultlimit,

        /// <summary>
        /// Limit at which relationship values must be searched for
        /// </summary>
        relationshipoptionlimit,

        /// <summary>
        /// Limit the number of containers allowed to receive in a single operation
        /// </summary>
        container_receipt_limit,

        /// <summary>
        /// The maximum number of lines in comments fields
        /// </summary>
        total_comments_lines,

        /// <summary>
        /// The name of the root level item on location views
        /// </summary>
        LocationViewRootName,

        /// <summary>
        /// Whether Inspections become 'Missed' when Generator creates new inspection, 1=true
        /// </summary>
        miss_outdated_inspections,

        /// <summary>
        /// 
        /// </summary>
        sql_report_resultlimit,

        /// <summary>
        /// Customer username for accessing ChemWatch
        /// </summary>
        chemwatchusername,

        /// <summary>
        /// Customer password for accessing ChemWatch
        /// </summary>
        chemwatchpassword,

        /// <summary>
        /// Customer level limit on Ariel regions.
        /// </summary>
        arielmodules,

        /// <summary>
        /// Domain for accessing ChemWatch
        /// </summary>
        chemwatchdomain,

        /// <summary>
        /// Number of previously used passwords to disallow
        /// </summary>
        password_reuse_count,

        mobileview_resultlim
    };

}