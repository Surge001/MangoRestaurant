WITH CTE ( dbName, hostname, last_sql, last_batch)
AS (
       SELECT 
       DB_NAME(dbid), 
       hostname, 
       (
                select text from sys.dm_exec_sql_text(S.sql_handle)
       ) as last_sql,
       last_batch
       FROM sys.sysprocesses S
       where dbid > 0
       --and program_name = '.Net SqlClient Data Provider'
)
select dbname, last_sql, min(last_batch) minLastBatch, max(last_batch) maxLastBatch, count(*) as CountBySQL from CTE
group by last_sql, dbname
order by count(*) desc;
