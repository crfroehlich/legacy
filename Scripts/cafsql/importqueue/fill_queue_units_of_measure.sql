insert into nbtimportqueue@CAFLINK ( nbtimportqueueid, state, itempk, sheetname, priority, errorlog)  select seq_nbtimportqueueid.nextval@CAFLINK, 'I', unitofmeasureid, 'weight_view',0, '' from weight_view@CAFLINK where deleted='0' 