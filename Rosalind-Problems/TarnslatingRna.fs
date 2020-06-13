namespace Rosalind_Problems

open System
open System.IO
open Rosalind_Problems.Helpers
open Xunit
open Xunit.Abstractions
open FSharp.Data
open RosalindLib

module TarnslatingRna =

    let keywordStop = "Stop"
    
    let getDataset =
        let workingDir = Path.Combine(Directory.GetCurrentDirectory(), "TestData")
        let path = Path.Combine(workingDir, "rna_codon_data_set.txt")
        FileUtilities.readLines path
            
    let matchAminoAcid rnaCodon =
        match rnaCodon with
        | "UUC" | "UUU" -> "F"
        | "UUA" | "UUG" | "CUC" | "CUU" | "CUA" | "CUG" -> "L"
        | "UCU" | "UCC" | "UCA" | "UCG" | "AGU" | "AGC" -> "S"
        | "UAU" | "UAC" -> "Y"
        | "UAA" | "UAG" | "UGA" -> keywordStop
        | "UGU" | "UGC" -> "C"
        | "UGG" -> "W"
        | "CCU" | "CCC" | "CCA" | "CCG" -> "P"
        | "CAU" | "CAC" -> "H"
        | "CAA" | "CAG" -> "Q"
        | "CGU" | "CGC" | "CGA" | "CGG" | "AGA" | "AGG" -> "R"
        | "AUC" | "AUU" | "AUA" -> "I"
        | "AUG" -> "M"
        | "ACU" | "ACC" | "ACA" | "ACG" -> "T"
        | "AAU" | "AAC" -> "N"
        | "AAA" | "AAG" -> "K"
        | "GUC" | "GUU" | "GUA" | "GUG" -> "V"
        | "GCU" | "GCC" | "GCA" | "GCG" -> "A"
        | "GAU" | "GAC" -> "D"
        | "GAA" | "GAG" -> "E"
        | "GGU" | "GGC" | "GGA" | "GGG" -> "G"
        | _ -> "Unknown"

    type RnaCodon = CsvProvider<"TestData/rna-codon-table.csv">

    let parseAminoAcids (rnaCodonString:string) =
        (StringUtilities.chunkString 3 rnaCodonString
        |> List.map (fun rc -> matchAminoAcid rc)
        |> String.concat "").Split(keywordStop)
        |> Array.filter(fun s -> s <> "")

    type TarnslatingRnaTests(output: ITestOutputHelper) =
        do new Converter(output) |> Console.SetOut
        
        [<Fact>]
        member __.``Verify RNACodon matches Amino Acid``() =
            let rnaCodonEntries = RnaCodon.Load("TestData/rna-codon-table.csv")
            for entry in rnaCodonEntries.Rows do
                Assert.Equal(matchAminoAcid entry.RnaCodon, entry.AminoAcid)
        
        [<Fact>]
        member __.``Parse RNA codons and match proteins.`` () =
            let aminoAcids = parseAminoAcids (getDataset |> Seq.toArray).[0]
            let expected = "MTPPNRLALTRCSLSINLAKNVARHRENIRAFTLFRNVLLSYGQVACKIHAAPTPDGESSRANTVLVSIIARGGNSADDQDCGRGAGARVFACPGLFVPGQRVNSRHQSLLTRGVHTPSLGPRKGSTLKRRACNAVWVSIPLMCPNAGGMRYAHQIYEHARQLRHAESCNTRRLFRTSADTNSVTQCVLATGIVPSRPSTPRLWTGRSFDPHSFARNMLALACGGSALILRKRCMHTPTVNQCLNSLRISTLLGRTRKLSTKETSITMWVTGTVHCAIAKVMLTLTLSNAGAPYHHGLGRNFLSTPPPVAQGEVAEPLPDPGDIVESSDLPFRIKVIYWRYKLIVDILTKKPDPAEPGFSRRGWIIALLDRLMKFRALSAVAVLPALSLLAVSQRNHIRLTEVLLCCSRTSNLRRGGQPGIGAKLTIARRTRLRERVRTKSPQAQRRMAAVTVLSALPMLNSVVLMIPFNLVHTGTDLVLCLLALWLFTDFSSNIKREGWDRKAKFTSDINPATTAVVRRLQKLWDIGVTPLTKRFAFASEESHLLLDHNETAGATINDTISQPCLRSGGHQYQNMCKCGVYSCGITGIKEHKARSIVVIGGANEQEKNRNTACALSARTGACRNPEKLLCRQYCHYLMREPNREELLRRALAAAYTGMDDNSVDLEGGPPTQPRKADYPLLRADQPGISSSSYSPPAQQTNEGLSRQPILPPNRGRPHGSYLGCGTTELRTTTEQVPVVKSGALSSAVAPIRSDWLIIELQYMVLRSPDLPTDLRRNHLGRQGSSRSSTVRIPSTQGSSSVRKVIFRSFLFCRAYRNYQLLSTLYRMTLWLCRTLYAPPVYLHTRPLERRQCGLGSEGTTLLRHITPNPSQEPQERYTVPLPTPRFGPHNAAPRVESNLNRCNTENPTMDTSSHISPRRSTGLRNAPGRVIRNGALFYRYHATGYIGVPISPGMQTFTHSAIAQTGNTNSHALHLTNPGIELNNPVLVTKKVPILRVRSKDGKQNATRPWEGHSSRAGERQNAECHVTRQRIISEFRALSNPLKSRYIPNIVLRWQLGTRIPHSSTPPTRRTSGVMYLLPSNLLRCGPWGSQLQKGRCALLNDRLLLLVVIMPLRRREQKVTYQGPQKSIVSSTTVGPRAHYTLTEYLEKTPAQETLLLMRTPLTPRTQPHLLALELHFYSFLALSSYLPRLKIHLYPRPVFDHCGRSLRRHYAHSGSDKSRTLFGRPPCPTARRHTEHRTVLPSVQHIQTGNSGSAMHIVQTWPRPFGAHSLLLCNNTAETLSPAQSIQRGHNGRLMNSHSGDDSKGYPSLGLLHKQRVSLDSRTGVYPHRTSRTKGGKQAEAKKSSLRNSSHITTLRAARVSLNMKVGTKSIGYWRRVGWVQFRERQVLRDAGRAEFQVGQRNHQLPTPLLPLLHEGRLEEFSSSTDSGFNYNPLLAYLISPPVFSFAQSRFASMRAAVAPAYVVPSIRLKAMDLQPLFVTSRSYSAWTSAPRTEAQSTSMIRHFFAFKPLYAANFDCRETRSITGVSAGGVTALSSGLSIRLTPPRIRNPSGRGGKFRGLWPWKNSPYQLTVYFGLVTHRFIPSAPLTVLIYQNFNLNGGIAHRRALCPDSNFAHSIAALIILRPRSQLSTTSLVHWSPALIRGSFSTPRVTRRQHYSGEVLGTNPRSHLRALGLLYQNSDRQPRPPGNGFPQRDLIRGSSWLGCRKIYCRNPITSLILRDTLRPRYNGLGCISGDVPRIKVALSEIWGGVVHQLTINPNPTLHVVTGRKPRIDWRSRGSLPSPVFMGRDVGHYVLKLRVASLTCLTWKPGQGRRQRTRMAEAPVASGYVQCWTYTYNPYIYLALATRYKFMQNITMATENHGPEHQQIQGSDLFPAFNHSIVSEMSFRSTELRTADARQQVTVWCTKSPPAERRLVTIFSRRGERAPRDIPVRASSIYYVPSIRHRGANSREFINVSGTHKGTIVATGSRPKRVFKTIILSSTMQAGNCKCPCLEKRFSRTKIFVDIQRGCLLGHRTPSLEPLKGLSRGWKRYLQLKLFTIIGRYNHDLHSGCAPSLIESTQLAVERRAGNTSFVLPMTFNERGDHSVYKPNTAVRHLGCLQYIIAPPSCRQFSSLGNYVYALWATRASPKVMMREHHQSQHLITPGRNSLETGSIGLHVFTRAFGRLDVYSHLHLFSLAILRGGEGIPATILLFMMGRVRVSYISRCRMSGTALSKAYTSQSVYQTKDWSLQYPLGTPNLVIYTGTGADVQSASRDMFRTTMDWITVFSQRVLASRRRGHLFVLSATDAESLPEIRAGCRSRAVYTTQGHRPLVYEALADSCYVIGAPCRYGMAYHDGRHATVRPPRGCPYLLHVHHHLWDTSLLFGQGSGQQVERSKPGHEAVIALLAAFALAVFCTSIIPSEYANCRAVLGYIRPTANPRRVTPGPERSFNLVPVIDWPSQRQAILSQLSNAFCSGTRRVVVCSGQTYIHSFYGEMLGGSFLPCATPFNYSGRQDGTTNPVTLKWDTHLLGISTTFYGQKWAAFAEVVGPSSGTRGCPSGVSRTSASGGTGTLVRGAYRDLFGDRFSGVLLPPVEFILRSVLAQPSQQFVTASAPRGRVLSTGPFNDPAPGLQAFLAPHSDCNADCGLIYFHGMGCYVQRCCSRAGMACINGVIGIALAADQETVILGNRWPANKTELESGLISKTGKFVRLLRRPAWTSLVSRLLADPTSAIVSHRDVASKWILHQTRVLHRTEPRRRVILLKHRNLAAACLVLGRSYLAAVIGTGLGFTHVEVAPSGDLLQTSSTCHFTLDIGLPHPGLRALSTYFGPIRLFHRIAPSIVLACLRPRELGRCMPRLLTSLPWVKNVINTVKKGCPECMNTIWITMRPAQYWITNRAVLNSQSLLILLNHTSLKNGKKRRKVRPSPQKSAPTVSIRYCPAGRVALEHVGAQCYTNRLRHAQKCSLDLPSQTAYKLGHKNHKTAGLSSQGSYPRIPGCITYLPAGRSELSGRIAAWKVCTVARFSLHTRVCPDLKPRTLAPINYIPGFVIPTRYYPELTVPENYFVIGNGLSNAPSICQFPRPVKELVIMFYAAATDITMGTVGYTNWYHDVFTERAPRPLPTVADRSRSRGEYNEMYLSLTTDENSTYYAYPYRYKLLISRLTPVTPVRDRIRWHRDTLDNALTSGMCIIPPLGSMVITIELHLSARCVGRSVESQIPDP"
            for ac in aminoAcids do
                printfn "%s" ac
            Assert.Equal(expected, aminoAcids.[0])
            