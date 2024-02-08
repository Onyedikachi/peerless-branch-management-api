using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.LedgerModule.Model
{

    public class LedgerResponseModel
    {
        public Links links { get; set; }
        public int total { get; set; }
        public int total_pages { get; set; }
        public int current_page { get; set; }
        public int page_size { get; set; }
        public List<LedgerResult> results { get; set; } = new();
    }

    public class Links
    {
        public object next { get; set; }
        public object previous { get; set; }
    }
    public class Ledgerbase
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool is_parent { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool post_no_debit { get; set; }
        public bool post_no_credit { get; set; }
        public string identifier { get; set; }
        public string ledger_code { get; set; }
        public string class_ledger_code { get; set; }
        public string parent_ledger_code { get; set; }
        public string sequential_ledger_code { get; set; }
    }

    public class LedgerResult:Ledgerbase
    {
      
        public int sub_ledger_sequencing_start { get; set; }
        public int sub_ledger_sequencing_stop { get; set; }
        public string sub_ledger_sequencing_type { get; set; }
        public int sub_ledger_sequencing_type_number { get; set; }
        public Initiator initiator { get; set; }
        public string approved_by { get; set; }
        public bool is_default { get; set; }
        public string state { get; set; }
        public bool is_locked { get; set; }
        public object group_id { get; set; }
        public Gl_Class gl_class { get; set; }
        public object parent_ledger { get; set; }
        public float net_balance { get; set; }
        public bool balance_flag { get; set; }
        public Currency currency { get; set; }
        public int transaction_count { get; set; }
        public Sub_Ledgers[] sub_ledgers { get; set; }
    }

    public class Initiator
    {
        public string id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public bool is_active { get; set; }
    }

    public class Gl_Class
    {
        public string id { get; set; }
        public string name { get; set; }
        public string debit_impact_on_balance { get; set; }
        public string credit_impact_on_balance { get; set; }
        public string code { get; set; }
        public int children_count { get; set; }
        public bool is_configured { get; set; }
    }

    public class Currency
    {
        public string id { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public bool is_default { get; set; }
    }

    public class Sub_Ledgers:Ledgerbase
    {
      
        public object sub_ledger_sequencing_start { get; set; }
        public object sub_ledger_sequencing_stop { get; set; }
        public object sub_ledger_sequencing_type { get; set; }
        public object sub_ledger_sequencing_type_number { get; set; }
        public Initiator1 initiator { get; set; }
        public string approved_by { get; set; }
        public bool is_default { get; set; }
        public string state { get; set; }
        public bool is_locked { get; set; }
        public object group_id { get; set; }
        public Gl_Class1 gl_class { get; set; }
        public Parent_Ledger parent_ledger { get; set; }
        public float net_balance { get; set; }
        public bool balance_flag { get; set; }
        public Currency1 currency { get; set; }
        public int transaction_count { get; set; }
        public object[] sub_ledgers { get; set; }
    }

    public class Initiator1
    {
        public string id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public bool is_active { get; set; }
    }

    public class Gl_Class1
    {
        public string id { get; set; }
        public string name { get; set; }
        public string debit_impact_on_balance { get; set; }
        public string credit_impact_on_balance { get; set; }
        public string code { get; set; }
        public int children_count { get; set; }
        public bool is_configured { get; set; }
    }

    public class Parent_Ledger
    {
        public string id { get; set; }
        public string name { get; set; }
        public string ledger_code { get; set; }
        public string ledger_code_format { get; set; }
    }

    public class Currency1
    {
        public string id { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public bool is_default { get; set; }
    }

}
