using CsvHelper.Configuration;
using StockMarket.Api.Models;

public class CompMap : ClassMap<Comp>
{
    public CompMap()
    {
        Map(m => m.CompCd).Name("comp_cd");
        Map(m => m.CompNm).Name("comp_nm");
        Map(m => m.SectMajCd).Name("sect_maj_cd");
        Map(m => m.SectMinCd).Name("sect_min_cd");
        Map(m => m.InstrCd).Name("instr_cd");
        Map(m => m.CatTp).Name("cat_tp");
        Map(m => m.Add1).Name("add1");
        Map(m => m.Add2).Name("add2");
        Map(m => m.RegOff).Name("reg_off");
        Map(m => m.PrnSth).Name("prn_sth");
        Map(m => m.OpnDt).Name("opn_dt");
        Map(m => m.TaxHday).Name("tax_hday");
        Map(m => m.Tel).Name("tel");
        Map(m => m.Tlx).Name("tlx");
        Map(m => m.EMail).Name("e_mail");
        Map(m => m.Prod).Name("prod");
        Map(m => m.ProVol).Name("pro_vol");
        Map(m => m.Spnr).Name("spnr");
        Map(m => m.AthoCap).Name("atho_cap");
        Map(m => m.PaidCap).Name("paid_cap");
        Map(m => m.NoShrs).Name("no_shrs");
        Map(m => m.FcVal).Name("fc_val");
        Map(m => m.Mlot).Name("mlot");
        Map(m => m.SbaseRt).Name("sbase_rt");
        Map(m => m.FlotDtFm).Name("flot_dt_fm");
        Map(m => m.FlotDtTo).Name("flot_dt_to");
        Map(m => m.BokClFdt).Name("bok_cl_fdt");
        Map(m => m.BokClTdt).Name("bok_cl_tdt");
        Map(m => m.Margin).Name("margin");
        Map(m => m.AvgRt).Name("avg_rt");
        Map(m => m.RtUpdDt).Name("rt_upd_dt");
        Map(m => m.Flag).Name("flag");
        Map(m => m.Auditor).Name("auditor");
        Map(m => m.NsIcb).Name("ns_icb");
        Map(m => m.NsUnit).Name("ns_unit");
        Map(m => m.NsMutual).Name("ns_mutual");
        Map(m => m.Pmargin).Name("pmargin");
        Map(m => m.RissuDtFm).Name("rissu_dt_fm");
        Map(m => m.RissuDtTo).Name("rissu_dt_to");
        Map(m => m.Premium).Name("premium");
        Map(m => m.Cflag).Name("cflag");
        Map(m => m.MarFloat).Name("mar_float");
        Map(m => m.MonTo).Name("mon_to");
        Map(m => m.TradeMeth).Name("trade_meth");
        Map(m => m.CseInstrCd).Name("cseinstr_cd");
        Map(m => m.IndxLst).Name("indx_lst");
        Map(m => m.BaseUpdDt).Name("base_upd_dt");
        Map(m => m.Cds).Name("cds");
        Map(m => m.CtlRt).Name("ctl_rt");
        Map(m => m.Net).Name("net");
        Map(m => m.Grp).Name("grp");
        Map(m => m.MerchanBankId).Name("merchan_bank_id");
        Map(m => m.Otc).Name("otc");
        Map(m => m.IpoCutoffDt).Name("ipo_cutoff_dt");
        Map(m => m.TradePlatform).Name("trade_platform");
        Map(m => m.PeRatio).Name("pe_ratio");
        Map(m => m.IsinCd).Name("isin_cd");
        Map(m => m.StartDt).Name("start_dt");
        Map(m => m.Ldrn).Name("ldrn");

        // Ignored properties
        Map(m => m.Id).Ignore();
        Map(m => m.SectMaj).Ignore();
        Map(m => m.Website).Ignore();
        Map(m => m.ListingYear).Ignore();
        Map(m => m.LastAgmHeld).Ignore();
        Map(m => m.EarningPerShare).Ignore();
        Map(m => m.NetAssetValPerShare).Ignore();
        Map(m => m.NocfPerShare).Ignore();
        Map(m => m.SharePercentageDirector).Ignore();
        Map(m => m.SharePercentageForeign).Ignore();
        Map(m => m.SharePercentageGovt).Ignore();
        Map(m => m.SharePercentageInstitute).Ignore();
        Map(m => m.SharePercentagePublic).Ignore();
        Map(m => m.YearEnd).Ignore();
        Map(m => m.OperationalStatus).Ignore();
        Map(m => m.Fax).Ignore();
    }
}