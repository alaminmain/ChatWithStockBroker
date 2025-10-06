
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using StockMarket.Api.Data;
using StockMarket.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMarket.Api.Services
{
    public class OracleDataService
    {
        private readonly string? _oracleConnectionString;
        private readonly ApplicationDbContext _context;

        public OracleDataService(IConfiguration configuration, ApplicationDbContext context)
        {
            _oracleConnectionString = configuration.GetConnectionString("OracleConnection");
            _context = context;
        }

        public async Task ImportMarPriceData()
        {
            if (string.IsNullOrEmpty(_oracleConnectionString))
            {
                throw new InvalidOperationException("Oracle connection string is not configured.");
            }
            var lastEntryDate = _context.MarPrices.Any() ? _context.MarPrices.Max(mp => mp.TransDt) : null;
            var query = @"SELECT 
               TRANS_DT AS TransDt, 
               INST_CD AS InstCd, 
               COMP_CD AS CompCd, 
               OPEN AS Open, 
               HIGH AS High, 
               LOW AS Low, 
               CLOSE AS Close, 
               CHG AS Chg, 
               VOL AS Vol, 
               VAL AS Val, 
               GRP AS Grp, 
               MARK_TP AS MarkTp, 
               AVRG_RT AS AvrgRt, 
               GEN_INDX AS GenIndx, 
               INDX_CHG AS IndxChg, 
               MARK_CAP AS MarkCap, 
               T_VAL AS TVal, 
               ISIN_CD AS IsinCd, 
               DSEX_INDX AS DsexIndx
            FROM INVEST.MAR_PRICE";

            if (lastEntryDate.HasValue)
            {
                query += " WHERE TRANS_DT > :lastEntryDate";
            }

            query += " ORDER BY TRANS_DT DESC NULLS LAST";

            using (var connection = new OracleConnection(_oracleConnectionString))
            {
                var marPrices = await connection.QueryAsync<MarPrice>(query, new { lastEntryDate });
                if (marPrices.Any())
                {
                    await _context.MarPrices.AddRangeAsync(marPrices);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
