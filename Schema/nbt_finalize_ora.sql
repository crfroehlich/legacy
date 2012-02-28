

CREATE OR REPLACE PROCEDURE update_sequences authid current_user IS
  cursor cur is select columnname, tablename from data_dictionary where columntype = 'pk' and isview= '0';

  cursor cur2 is select t.sequenceid, replace(t.sequencename, ' ', '') sequencename, x.maxval
                   from sequences t
                   join (select s.sequenceid,
                                (nvl(max(j.field1_numeric),0) + 1) maxval
                           from sequences s
                           left outer join nodetype_props p on (s.sequenceid = p.sequenceid)
                           left outer join jct_nodes_props j on (p.nodetypepropid = j.nodetypepropid)
                          group by s.sequenceid) x on x.sequenceid = t.sequenceid;

  var_maxval NUMBER;
  var_seqval NUMBER;
  var_seqcnt number;
  begin

  -- note: system must:
  --  grant execute on this_user.update_sequences to this_user;
  --  grant create any sequence to this_user;

  -- Table Sequences
  for rec in cur loop
    execute immediate 'select count(*) from user_sequences where lower(sequence_name)=lower(''seq_' || rec.columnname || ''')' into var_seqcnt;

    if(var_seqcnt<1) then
      execute immediate 'create sequence seq_' || rec.columnname || ' minvalue 1 start with 1 increment by 1';
    end if;

    execute immediate 'select nvl(max(' || rec.columnname || '), 1) from ' || rec.tablename into var_maxval;
    execute immediate 'SELECT NVL(last_number,0) FROM user_sequences WHERE lower(sequence_name)=lower(''seq_' || rec.columnname || ''')' into var_seqval;

    if var_maxval >= var_seqval then
      execute immediate 'alter sequence seq_' || rec.columnname || ' increment by ' || to_char(greatest(var_maxval - var_seqval + 1, 1)) || ' ';
      execute immediate 'select seq_' || rec.columnname || '.nextval from dual' into var_seqval;
      execute immediate 'alter sequence seq_' || rec.columnname || ' increment by 1';
    end if;
  end loop;
  
  -- NBT Sequences
  for rec2 in cur2 loop
    execute immediate 'select count(*) from user_sequences where lower(sequence_name)=lower(''seq_' || rec2.sequencename || ''')' into var_seqcnt;

    if(var_seqcnt<1) then
      execute immediate 'create sequence seq_' || rec2.sequencename || ' minvalue 1 start with 1 increment by 1';
    end if;

    execute immediate 'SELECT NVL(last_number,0) FROM user_sequences WHERE lower(sequence_name)=lower(''seq_' || rec2.sequencename || ''')' into var_seqval;

    if rec2.maxval is not null and rec2.maxval >= var_seqval then
      execute immediate 'alter sequence seq_' || rec2.sequencename || ' increment by ' || to_char(greatest(rec2.maxval - var_seqval + 1, 1)) || ' ';
      execute immediate 'select seq_' || rec2.sequencename || '.nextval from dual' into var_seqval;
      execute immediate 'alter sequence seq_' || rec2.sequencename || ' increment by 1';
    end if;

  end loop;
  end;

/

exec update_sequences;
commit;


create or replace procedure DROP_SEQUENCES is
begin
  DECLARE
    CURSOR cur_objects(obj_type VARCHAR2) IS
      SELECT object_name FROM user_objects WHERE object_type IN (obj_type);

    obj_name VARCHAR(200);
    sql_str  VARCHAR(500);

  BEGIN
    OPEN cur_objects('SEQUENCE');

    LOOP
      FETCH cur_objects
        INTO obj_name;
      EXIT WHEN cur_objects%NOTFOUND;

      sql_str := 'drop SEQUENCE ' || obj_name;
      EXECUTE IMMEDIATE sql_str;

    END LOOP;

    CLOSE cur_objects;
  END;

end;
/



create or replace view nbtdata as
select n.nodeid, n.nodename,  t.nodetypeid, t.nodetypename, o.objectclassid, o.objectclass,
p.nodetypepropid, p.propname, op.objectclasspropid, op.propname objectclasspropname, f.fieldtype,
j.jctnodepropid, j.field1, j.field2, j.field3, j.field4, j.field5, j.field1_fk, j.gestalt, j.clobdata
from nodes n
join nodetypes t on n.nodetypeid = t.nodetypeid
join object_class o on t.objectclassid = o.objectclassid
join nodetype_props p on p.nodetypeid = t.nodetypeid
join field_types f on p.fieldtypeid = f.fieldtypeid
left outer join object_class_props op on p.objectclasspropid = op.objectclasspropid
left outer join jct_nodes_props j on (j.nodeid = n.nodeid and j.nodetypepropid = p.nodetypepropid)
order by lower(n.nodename), lower(p.propname);

commit;


--case 22692 (dch 2/3/2012)
create or replace function alnumOnly(inputStr in varchar2, replaceWith in varchar2) return varchar2 is
  Result varchar2(1000);
begin
  
  Result := regexp_replace(inputStr,'[[:space:]]', null );

  Result := regexp_replace(trim(Result), '[^a-zA-Z0-9_]', replaceWith ); 
  
  return(Result);
end alnumOnly;
/
commit;

create or replace function OraColLen(aprefix in varchar2,inputStr in varchar2,asuffix in varchar2) return varchar2 is
  Result varchar2(100);
  maxlen number(3);
begin
  --dbms_output.put_line(aprefix || inputstr || asuffix);
  maxlen := 28- nvl(length(aprefix),0) - nvl(length(asuffix),0);
  --dbms_output.put_line(maxlen);
  Result := aprefix || substr(trim(inputStr),1,maxlen) || asuffix; 
  --dbms_output.put_line(Result);
  return(Result);
end OraColLen;
/
commit;


create or replace view vwAutoViewColNames as 
select n.nodetypename,p.propname,'NT' || upper(alnumonly(n.nodetypename,'')) viewname, 
case
 when s.subfieldname is null then 'P' || to_char(p.firstpropversionid) 
 when s.subfieldname is not null and fktype='ObjectClassId' then 'P' || to_char(p.firstpropversionid) || '_' || alnumonly(ocfk.objectclass,'') || '_OCFK'
 when s.subfieldname is not null and fktype='NodeTypeId' then 'P' || to_char(p.firstpropversionid) || '_' || alnumonly(ntfk.nodetypename,'') || '_NTFK'
 else 'P' || to_char(p.firstpropversionid) || '_' || s.subfieldname
end colname
from nodetype_props p
 join nodetypes n on n.nodetypeid=p.nodetypeid
 join object_class c on c.objectclassid=n.objectclassid
 join field_types f on f.fieldtypeid=p.fieldtypeid
 join field_types_subfields s on s.fieldtypeid=f.fieldtypeid and s.reportable='1'
 left outer join nodetypes ntfk on ntfk.nodetypeid=p.fkvalue and p.fktype='NodeTypeId'
 left outer join object_class ocfk on ocfk.objectclassid=p.fkvalue and p.fktype='ObjectClassId' 
order by n.nodetypename,p.propname,s.subfieldname;
/
commit;

create or replace view vwntpropdefs as
select ntp.nodetypeid,ntp.propname,ntp.firstpropversionid nodetypepropid,ft.fieldtype,ft.fieldtypeid,ntp.fktype,ntp.fkvalue,
  nt.nodetypename,oc.objectclass 
 from nodetype_props ntp
 join field_types ft on ft.fieldtypeid=ntp.fieldtypeid and ft.deleted='0'
 left outer join nodetypes nt on nt.nodetypeid=ntp.fkvalue and ntp.fktype='NodeTypeId'
 left outer join object_class oc on oc.objectclassid=ntp.fkvalue and ntp.fktype='ObjectClassId';
/ 
commit;

create or replace view vwnpv as
select j.nodeid nid,to_char(j.gestalt) gestalt,
  field1_fk,field1_date,field2_date,field1_numeric,field2_numeric,
  field1,field2,field3,field4,to_char(clobdata) clobdata,
  nodetypepropid ntpid from jct_nodes_props j;
/
commit;

create or replace view vwnpvname as
select j.nodeid nid,to_char(j.gestalt) gestalt,field1_fk,j.nodetypepropid ntpid,ntp.propname from jct_nodes_props j
 join nodetype_props ntp on ntp.nodetypepropid=j.nodetypepropid;
/
commit;

create or replace view vwobjprops as
select distinct ntp.propname,'gestalt' subfieldname,nt.objectclassid,'' subfieldalias
 from nodetype_props ntp
 join nodetypes nt on nt.nodetypeid=ntp.nodetypeid
/
commit;



create or replace procedure createNTview(ntid in number) is
  cursor props is
  select v.*,s.propcolname subfieldname,s.is_default,s.subfieldname subfieldalias from vwNtPropDefs v
    join field_types_subfields s on s.fieldtypeid=v.fieldtypeid and s.reportable='1'
    where v.nodetypeid=ntid order by lower(propname);
  var_sql varchar2(32000);
  var_line varchar(2000);
  pname varchar2(200);
  pcount number;
  viewname varchar2(30);
  objid number;
  ntcount number;
begin
  dbms_output.enable(32000);


 select count(*) into ntcount from nodetypes where nodetypeid=ntid;
  
 if(ntcount>0) then
  select nodetypename,objectclassid into viewname,objid from nodetypes where nodetypeid=ntid;

  var_line:='create or replace view ' || OraColLen('NT',alnumonly(viewname,''),'') || ' as select n.nodeid ';
  --dbms_output.put_line(var_line);
  var_sql := var_sql || var_line;
  pcount:=0;

  for rec in props loop
      pcount:=pcount+1;
      --dbms_output.put_line(to_char(pcount) || '|' || rec.propname || '|' || rec.subfieldname || '|' || rec.fieldtype || '|' || rec.objectclass || '|' || rec.nodetypename);
      pname := to_char(rec.nodetypepropid);
      if(rec.is_default='1') then
        var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid);
        var_line := var_line || ')' || OraColLen('P',alnumonly(upper(pname),''),'');
        --dbms_output.put_line(var_line);
        var_sql := var_sql || var_line;
      else
        if(rec.fieldtype='Relationship' or rec.fieldtype='Location') then
          if(rec.fktype='NodeTypeId') then
            var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid) || ') ';
            var_line := var_line  || OraColLen('P',alnumonly(upper(pname || '_' || rec.nodetypename),''),'_NTFK');
          --  dbms_output.put_line(var_line);
            var_sql := var_sql || var_line;
          else
            var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid) || ') ';
            var_line:=var_line || OraColLen('P',alnumonly(upper(pname || rec.objectclass),''),'_OCFK');
          --  dbms_output.put_line(var_line);
            var_sql := var_sql || var_line;
          end if;
        else
          var_line := ',(select ' || rec.subfieldname || ' from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.nodetypepropid) || ') ';
          var_line:=var_line || OraColLen('P',alnumonly(upper(pname || '_' || rec.subfieldalias),''),'');
          --dbms_output.put_line(var_line);
          var_sql := var_sql || var_line;
        end if;
      end if;
  end loop;

  if(pcount>1) then
    var_line := ' from nodes n where n.nodetypeid=' || to_char(ntid);
    --dbms_output.put_line(var_line);
    var_sql := var_sql || var_line;
    execute immediate (var_sql);
    commit;
    createOCview(objid);
  end if;
 end if;

end createNTview;
/

commit;



create or replace procedure createOCview(objid in number) is
  cursor props is 
  select v.* from vwObjProps v 
    where v.objectclassid=objid order by lower(propname);
  var_sql varchar2(20000);
  aline varchar(2000);
  pcount number;
  pname varchar2(200);
  viewname varchar(30);
begin
  --dbms_output.enable(21000);
  select objectclass into viewname from object_class where objectclassid=objid;  

  var_sql:='create or replace view ' || OraColLen('OC',alnumonly(viewname,''),'') || ' as select n.nodeid ';
  --dbms_output.put_line(var_sql);
  pcount:=0;
  
  for rec in props loop
      pcount:=pcount+1;
      pname:=alnumOnly(rec.propname,'');
      aline:=',(select ' || rec.subfieldname || ' from vwNpvname where nid=n.nodeid and propname=''' || rec.propname || ''') ' || OraColLen('P' || trim(to_char(pcount,'00')) || '_',alnumonly(upper(pname),''),'');  
      --dbms_output.put_line(aline);
      var_sql:=var_sql || aline;
  end loop;
  
  if(pcount>1) then 
    aline:= ' from nodes n join nodetypes nt on nt.nodetypeid=n.nodetypeid and nt.objectclassid=' || to_char(objid);
      --dbms_output.put_line(aline);
      var_sql:=var_sql || aline;
    execute immediate (var_sql);
    commit;
  end if;

end createOCview;
/

commit;

create or replace procedure CreateAllNtViews is
  cursor nts is
        select nodetypeid,nodetypename,firstversionid from nodetypes 
        where firstversionid=nodetypeid order by lower(nodetypename);
  cursor ntsdel is
	select object_name from user_objects where object_type='VIEW' and object_name like 'NT%' or object_name like 'OC%';
  var_sql varchar2(200);	  
begin
  for delrec in ntsdel loop
	var_sql := 'drop view ' || delrec.object_name;
	execute immediate (var_sql);	
  end loop;
  commit;

  for rec in nts loop
    --dbms_output.put_line('createntview(' || to_char(rec.nodetypeid) || ',' || rec.nodetypename || ')');
    CreateNtView(rec.nodetypeid);
  end loop;
end CreateAllNtViews;
/
commit;


--
--TEST: begin creation of views...
--
--set serveroutput on;

exec CreateAllNtViews();

commit;

--end case 22962
