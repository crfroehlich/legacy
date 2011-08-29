;(function($){
/**
 * jqGrid Hungarian Translation
 * Årszigety ÃdÃ¡m udx6bs@freemail.hu
 * http://trirand.com/blog/ 
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
**/

$.jgrid = {
	defaults : {
		recordtext: "Oldal {0} - {1} / {2}",
		emptyrecords: "Nincs talÃ¡lat",
		loadtext: "BetÃ¶ltÃ©s...",
		pgtext : "Oldal {0} / {1}"
	},
	search : {
		caption: "KeresÃ©s...",
		Find: "Keres",
		Reset: "AlapÃ©rtelmezett",
		odata : ['egyenlÅ‘', 'nem egyenlÅ‘', 'kevesebb', 'kevesebb vagy egyenlÅ‘','nagyobb','nagyobb vagy egyenlÅ‘', 'ezzel kezdÅ‘dik','nem ezzel kezdÅ‘dik','tartalmaz','nem tartalmaz','vÃ©gzÅ‘dik','nem vÃ©gzÅ‘dik','tartalmaz','nem tartalmaz'],
		groupOps: [	{ op: "AND", text: "all" },	{ op: "OR",  text: "any" }	],
		matchText: " match",
		rulesText: " rules"
	},
	edit : {
		addCaption: "Ãšj tÃ©tel",
		editCaption: "TÃ©tel szerkesztÃ©se",
		bSubmit: "MentÃ©s",
		bCancel: "MÃ©gse",
		bClose: "BezÃ¡rÃ¡s",
		saveData: "A tÃ©tel megvÃ¡ltozott! TÃ©tel mentÃ©se?",
		bYes : "Igen",
		bNo : "Nem",
		bExit : "MÃ©gse",
		msg: {
			required:"KÃ¶telezÅ‘ mezÅ‘",
			number:"KÃ©rjÃ¼k, adjon meg egy helyes szÃ¡mot",
			minValue:"Nagyobb vagy egyenlÅ‘nek kell lenni mint ",
			maxValue:"Kisebb vagy egyenlÅ‘nek kell lennie mint",
			email: "hibÃ¡s emailcÃ­m",
			integer: "KÃ©rjÃ¼k adjon meg egy helyes egÃ©sz szÃ¡mot",
			date: "KÃ©rjÃ¼k adjon meg egy helyes dÃ¡tumot",
			url: "nem helyes cÃ­m. ElÅ‘tag kÃ¶telezÅ‘ ('http://' vagy 'https://')",
			nodefined : " nem definiÃ¡lt!",
			novalue : " visszatÃ©rÃ©si Ã©rtÃ©k kÃ¶telezÅ‘!!",
			customarray : "Custom function should return array!",
			customfcheck : "Custom function should be present in case of custom checking!"
			
		}
	},
	view : {
		caption: "TÃ©tel megtekintÃ©se",
		bClose: "BezÃ¡rÃ¡s"
	},
	del : {
		caption: "TÃ¶rlÃ©s",
		msg: "KivÃ¡laztott tÃ©tel(ek) tÃ¶rlÃ©se?",
		bSubmit: "TÃ¶rlÃ©s",
		bCancel: "MÃ©gse"
	},
	nav : {
		edittext: "",
		edittitle: "TÃ©tel szerkesztÃ©se",
		addtext:"",
		addtitle: "Ãšj tÃ©tel hozzÃ¡adÃ¡sa",
		deltext: "",
		deltitle: "TÃ©tel tÃ¶rlÃ©se",
		searchtext: "",
		searchtitle: "KeresÃ©s",
		refreshtext: "",
		refreshtitle: "FrissÃ­tÃ©s",
		alertcap: "FigyelmeztetÃ©s",
		alerttext: "KÃ©rem vÃ¡lasszon tÃ©telt.",
		viewtext: "",
		viewtitle: "TÃ©tel megtekintÃ©se"
	},
	col : {
		caption: "Oszlopok kivÃ¡lasztÃ¡sa",
		bSubmit: "Ok",
		bCancel: "MÃ©gse"
	},
	errors : {
		errcap : "Hiba",
		nourl : "Nincs URL beÃ¡llÃ­tva",
		norecords: "Nincs feldolgozÃ¡sra vÃ¡rÃ³ tÃ©tel",
		model : "colNames Ã©s colModel hossza nem egyenlÅ‘!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0,00'},
		currency : {decimalSeparator:",", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0,00'},
		date : {
			dayNames:   [
				"Va", "HÃ©", "Ke", "Sze", "CsÃ¼", "PÃ©", "Szo",
				"VasÃ¡rnap", "HÃ©tfÅ‘", "Kedd", "Szerda", "CsÃ¼tÃ¶rtÃ¶k", "PÃ©ntek", "Szombat"
			],
			monthNames: [
				"Jan", "Feb", "MÃ¡r", "Ãpr", "MÃ¡j", "JÃºn", "JÃºl", "Aug", "Szep", "Okt", "Nov", "Dec",
				"JanuÃ¡r", "FebruÃ¡r", "MÃ¡rcius", "Ãprili", "MÃ¡jus", "JÃºnius", "JÃºlius", "Augusztus", "Szeptember", "OktÃ³ber", "November", "December"
			],
			AmPm : ["de","du","DE","DU"],
			S: function (j) {return '.-ik';},
			srcformat: 'Y-m-d',
			newformat: 'Y/m/d',
			masks : {
				ISO8601Long:"Y-m-d H:i:s",
				ISO8601Short:"Y-m-d",
				ShortDate: "Y/j/n",
				LongDate: "Y. F hÃ³ d., l",
				FullDateTime: "l, F d, Y g:i:s A",
				MonthDay: "F d",
				ShortTime: "a g:i",
				LongTime: "a g:i:s",
				SortableDateTime: "Y-m-d\\TH:i:s",
				UniversalSortableDateTime: "Y-m-d H:i:sO",
				YearMonth: "Y, F"
			},
			reformatAfterEdit : false
		},
		baseLinkUrl: '',
		showAction: '',
		target: '',
		checkbox : {disabled:true},
		idName : 'id'
	}
};
})(jQuery);
