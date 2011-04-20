-- drop all tables
drop table configuration_variables;
drop table fkey_definitions;
drop table jct_modules_actions;
drop table jct_modules_objectclass;
drop table jct_nodes_props;
drop table jct_nodes_props_audit;
drop table modules;
drop table nodes_audit;
drop table nodetype_props;
drop table nodetype_props_audit;
drop table nodetype_tabset;
drop table nodetypes_audit;
drop table object_class_props;
drop table sequences;
drop table static_sql_selects;
drop table statistics_actions;
drop table statistics_nodetypes;
drop table statistics_reports;
drop table statistics_searches;
drop table statistics_views;
drop table data_dictionary;
drop table field_types;
drop table license;
drop table license_accept;
drop table actions;
drop table node_views;
drop table nodes;
drop table nodetypes;
drop table object_class;
drop table statistics;
drop table update_history;
drop table jct_dd_ntp;
drop table packages;
drop table vendors;
drop table users;
drop table materials;
drop table materials_subclass;
drop table materials_synonyms;
drop table packdetail;
drop table locations;
drop table units_of_measure;
drop table containers;
drop table inventory_groups;
drop table sessionlist;
drop table welcome;
drop table jct_modules_nodetypes;
drop table schedule_items;
drop table scheduledrules;
drop table scheduledruleparams;
commit;

exec drop_sequences;
commit;
