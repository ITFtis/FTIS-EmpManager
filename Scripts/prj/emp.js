$(document).ready(function () {
    //清單欄位排序(非預設編輯頁)
    //說明: js, controller不可設Index排序(douoptions.fields已實體，順序不再變動)
    if (!douoptions.singleDataEdit) {
        //順序設定
        var ranks = [];
        ranks.push({ field: 'UpdateTime', index: '4' });
        //ranks.push({ field: 'Sex', index: '3' });

        var newFields = [];
        for (var i = 0; i < douoptions.fields.length; i++) {
            var rank = ranks.find(obj => obj.index == i);
            if (rank != null) {

                //指定(rank)欄位Field
                var f = douoptions.fields.find(obj => obj.field == rank.field);
                newFields.push(f);

                //原欄位Field
                newFields.push(douoptions.fields[i]);
            }
            else {
                if (ranks.find(obj => obj.field == douoptions.fields[i].field) == null) {
                    //原欄位
                    newFields.push(douoptions.fields[i]);
                }
            }
        }
        douoptions.fields = newFields;
    }

    var aryCheck = [];

    //20230811, add by markhong 分頁筆數設定
    douoptions.tableOptions.pageList = [10, 25, 50, 100, 'All'];

    douoptions.title = '員工資料';

    var $_d1EditDataContainer = undefined;      //Da1s編輯的容器
    var $_d4EditDataContainer = undefined;    //Da4s編輯的容器
    var $_d5EditDataContainer = undefined;    //Da5s編輯的容器
    var $_d6EditDataContainer = undefined;    //Da6s編輯的容器
    var $_d7EditDataContainer = undefined;    //Da7s編輯的容器
    var $_d8EditDataContainer = undefined;    //Da8s編輯的容器
    var $_d9EditDataContainer = undefined;    //Da9s編輯的容器

    var $_d1Table = undefined;  //Da1s Dou實體
    var $_d4Table = undefined;  //Da4s Dou實體
    var $_d5Table = undefined;  //Da5s Dou實體
    var $_d6Table = undefined;  //Da6s Dou實體
    var $_d7Table = undefined;  //Da7s Dou實體
    var $_d8Table = undefined;  //Da8s Dou實體
    var $_d9Table = undefined;  //Da9s Dou實體


    //主表(EmpData) 員工資料
    douoptions.afterCreateEditDataForm = function ($container, row) {

        var isAdd = JSON.stringify(row) == '{}';

        var $_oform = $("#_tabs");
        $_d1EditDataContainer = $('<div>').appendTo($_oform.parent());
        $_d4EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d5EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d6EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d7EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d8EditDataContainer = $('<table>').appendTo($_oform.parent());
        $_d9EditDataContainer = $('<table>').appendTo($_oform.parent());

        var oFno = row.Fno;
        var isChange = false;
        var isChangeText = [];

        //保留確定按鈕
        $container.find('.modal-footer button').hide();
        $container.find('.modal-footer').find('.btn-primary').show();

        if (!isAdd) {            
            //主表新增集合沒資料(預設集合)
            var iniObj = { Fno: row.Fno };

            //1-1 Detail(EmpDa1) 通訊方式            
            if (row.Da1s == undefined) {
                row.Da1s = iniObj;
            }
            SetDouEmpDa1(row.Da1s);

            //1-n Detail(EmpDa4) 學歷
            SetDouEmpDa4(row.Da4s, oFno);

            //1-n Detail(EmpDa5) 經歷
            SetDouEmpDa5(row.Da5s, oFno);

            //1-n Detail(EmpDa6) 家庭狀況
            SetDouEmpDa6(row.Da6s, oFno);

            //1-n Detail(EmpDa7) 外語檢定
            SetDouEmpDa7(row.Da7s, oFno);

            //1-n Detail(EmpDa8) 專業資格
            SetDouEmpDa8(row.Da8s, oFno);

            //1-n Detail(EmpDa9) 著作
            SetDouEmpDa9(row.Da9s, oFno);
        }

        //產tab        
        helper.bootstrap.genBootstrapTabpanel($_d4EditDataContainer.parent(), undefined, undefined,
            ['員工資料', '通訊方式', '學歷', '經歷', '家庭狀況', '外語檢定', '專業資格', '著作'],
            [$_oform, $_d1EditDataContainer, $_d4EditDataContainer, $_d5EditDataContainer, $_d6EditDataContainer, $_d7EditDataContainer, $_d8EditDataContainer, $_d9EditDataContainer]);


        //點選的Tab
        var jTabToggle = $('#_tabs').closest('div[class=tab-content]').siblings().find('a[data-toggle="tab"]');

        if (isAdd) {
            //tablist隱藏
            $('#_tabs').closest('div[class=tab-content]').siblings().hide();
        }

        //修改按鈕設定
        var editBtn = '';

        //按鈕「返回會員資料總表」
        if ($_masterTable != undefined) {
            if (isAdd) {
                //長在footer右邊
                var back = '<button id="btnBack" class="btn btn-secondary">返回會員資料總表</button>';
                $(back).appendTo($container.find('.modal-footer'));
            }
            else {
                //長在Tab右邊
                var back = '<button id="btnBack" class="btn btn-secondary">返回會員資料總表</button>';
                editBtn = back;                
            }            
        }

        if (!isAdd) {
            var btnExportBasicExcel = '<button id="btnExportBasicExcel" class="btn btn-primary ms-2">基本資料(Excel)</button>';
            var btnExportBasicWord = '<button id="btnExportBasicWord" class="btn btn-primary ms-2">基本資料(Word)</button>';
            var btnExportCV = '<button id="btnExportCV" class="btn btn-primary ms-2">履歷表</button>';
            editBtn = editBtn + btnExportBasicExcel + btnExportBasicWord + btnExportCV;

            editBtn = '<li class="ms-auto">' + editBtn  + '</li>';
            $(editBtn).appendTo($('#_tabs').closest('div[class=tab-content]').siblings());
        }

        $('#btnBack').click(function () {
            location.reload();
        });

        $('#btnExportBasicExcel').click(function () {
            var url = app.siteRoot + 'EmpBasic/ExportBasicExcel';
            ExportBasic(oFno, url)
        });

        $('#btnExportBasicWord').click(function () {
            var url = app.siteRoot + 'EmpBasic/ExportBasicWord';
            ExportBasic(oFno, url)
        });

        function ExportBasic(fno, url) {            

            helper.misc.showBusyIndicator();
            $.ajax({
                url: url,
                datatype: "json",
                type: "Get",
                data: { fno: fno },
                success: function (data) {
                    if (data.result) {
                        location.href = app.siteRoot + data.url;
                        //window.open(app.siteRoot + data.url, 'Download');
                    } else {
                        alert("產出基本資料表失敗：\n" + data.errorMessage);
                    }
                },
                complete: function () {
                    helper.misc.hideBusyIndicator();
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    alert(err.Message);
                    helper.misc.hideBusyIndicator();
                }
            });
        }

        $('#btnExportCV').click(function () {
            var fno = oFno;

            helper.misc.showBusyIndicator();
            $.ajax({
                url: app.siteRoot + 'EmpBasic/ExportCV',
                datatype: "json",
                type: "Get",
                data: { fno: fno },
                success: function (data) {
                    if (data.result) {
                        location.href = app.siteRoot + data.url;
                    } else {
                        alert("產出履歷表失敗：\n" + data.errorMessage);
                    }
                },
                complete: function () {
                    helper.misc.hideBusyIndicator();
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    alert(err.Message);
                    helper.misc.hideBusyIndicator();
                }
            });
        });

        //加提示字
        var $p1 = $('div[data-field=En_Name]').find('label');
        var remind = '<span class="text-danger fw-lighter pull-right">同護照英文姓名</span>';
        $(remind).appendTo($p1);

        //(tab)切換前
        jTabToggle.on('hide.bs.tab', function (e) {
            isChange = false;
            isChangeText = [];

            //當下tab item
            var $_nowTabUI = null;    //當下Tab資料 使用的UI;
            var $_nowTable = null;    //當下Tab資料 使用的Dou實體;

            var actTab = $(this).html();
            if (actTab == $_masterTable.instance.settings.title) {
                $_nowTable = $_masterTable;
                $_nowTabUI = $('#_tabs').closest('div[class=tab-content]').find('.show');
            }
            else if (actTab == $_d1Table.instance.settings.title) {
                $_nowTable = $_d1Table;
                $_nowTabUI = $('#_tabs').closest('div[class=tab-content]').find('.show');
            }
            else {
                //不需異動比對(1-n)
                return true;
            }

            //input:輸入值(UI) 容器
            var $_nowContainer = $_nowTabUI.children(":first").toggleClass("data-edit-form-group");

            //欄位驗證成功，錯誤內容關閉
            $(".errormsg", $_nowContainer.find('.modal-dialog')).hide().empty();


            if ($_nowTabUI == null || $_nowTable == null) {
                alert('當下Tab資料取得失敗');
                return false;
            }

            //停止Tab切換
            var tabStop = false;

            //input:輸入值(dou實體)
            var rdataDou = $_nowTable.instance.getData().find(obj => obj.Fno == oFno);

            //異動比對
            $.each($_nowTable.instance.settings.fields, function () {
                //欄位名稱
                var fn = this.field;

                //input:輸入值(UI)                
                var uiValue = douHelper.getDataEditContentValue($_nowContainer, this);

                //不需輸入值(UI)
                if (uiValue == null)
                    return; // 等於continue                

                //驗證：必填欄位
                if (!this.allowNull) {
                    if (uiValue == '') {
                        var errors = [];

                        errors.push(this.title + ":" + this.validate(uiValue, rdataDou));                        
                        var _emsgs = $.isArray(errors) ? errors : [errors];
                        $_nowContainer.find('.modal-dialog').trigger("set-error-message", '<span class="' + $_nowTable.instance.settings.buttonClasses.error_message + '" aria-hidden="true"></span>&nbsp; ' + _emsgs.join('<br><span class="' + $_nowTable.instance.settings.buttonClasses.error_message + '" aria-hidden="true"></span>&nbsp; '));
                        $_nowContainer.find('.modal-dialog').show();
                        $('html,body').animate({ scrollTop: $_masterTable.offset().top }, "show");

                        tabStop = true;
                        return false;
                    }
                }

                //input:輸入值(dou實體)
                var conValue = rdataDou[fn];

                if (conValue != null) {

                    //格式轉換
                    if (uiValue == "") {
                        //有異動不需轉換格式(執行:ui從有值改無值，切換tab)
                    }
                    else if (conValue != "") {
                        //日期格式比對(ui(1982-12-17), con(1982-12-17T00:00:00) => 取小統一長度)
                        if (this.datatype == 'datetime' || this.datatype == 'date') {
                            conValue = JsonDateStr2Datetime(conValue).DateFormat("yyyy/MM/dd HH:mm:ss");
                            var minLength = Math.min(uiValue.length, conValue.length);

                            uiValue = uiValue.substring(0, minLength)

                            //容器(時間可能是物件) "/Date(1224043200000)/"                                
                            conValue = conValue.substring(0, minLength);
                        }
                        else if (this.datatype == 'textarea') {
                            //換行資料庫:\r\n (備:number型別replace錯誤)
                            conValue = conValue.replaceAll('\r\n', '\n');
                        }
                    }

                    if (uiValue != conValue) {
                        isChange = true;
                        isChangeText.push(this.title);
                        //return false;
                    }
                }
                else {
                    //(異動說明)DB為Null($_nowTable無欄位資料),但UI有值
                    if (uiValue != "") {
                        isChange = true;
                        isChangeText.push(this.title);
                        //return false;
                    }
                }

            });

            //停止Tab切換(原因：必填欄位....等問題)
            if (tabStop) {
                return false;
            }

            //異動處理
            if (isChange) {

                //互動訊息
                var content = '資料異動(' + $_nowTable.instance.settings.title + ')項目：' + '</br>'
                    + isChangeText.join(', ') + '</br>'
                    + "是否儲存";

                //confirm挑選取消(重複執行，不知原因)
                var isDoing = false;

                //comfirm(確定儲存,取消還原)
                jspConfirmYesNo($("body"), { content: content }, function (confrim) {
                    if (confrim) {
                        //確定
                        $_nowTabUI.find('.modal-footer').find('.btn-primary').trigger("click");
                    }
                    else {
                        //取消
                        if (isDoing)
                            return;

                        //取消會轉回清單，不可用
                        //$_nowTabUI.find('.modal-footer').find('.btn-default').trigger("click");

                        //還原上一個Tab編輯資料
                        //input:輸入值(Bootstrap Table找編輯資料)
                        var $_bootstrapTable;
                        $('.bootstrap-table #_table').find('.dou-field-Fno').each(function (index) {
                            if ($(this).text() == oFno) {
                                $_bootstrapTable = $(this).closest("tr");
                                return false;
                            }
                        });

                        //input:輸入值(dou實體)
                        var rdataDou = $_nowTable.instance.getData().find(obj => obj.Fno == oFno);

                        //還原資料異動
                        $_nowTabUI.find('.field-content [data-fn]').each(function (index) {
                            //欄位名稱
                            var fn = $(this).attr('data-fn');
                            var datatype = douHelper.getField($_nowTable.instance.settings.fields, fn).datatype;

                            //輸入值(Bootstrap Table + dou實體)
                            var conValue = '';
                            if (datatype == 'textlist') {
                                //UI(人名)，bootstrapTable(人名)，X容器(員編)
                                conValue = $_bootstrapTable.find('.dou-field-' + fn).text();
                            }
                            else {
                                //var conValue = $_nowTable.instance.getData()[0][fn];
                                conValue = rdataDou[fn];
                            }

                            //conValue(null => DB欄位值Null) ("-" => 輸入提示字：-(沒值))
                            if (conValue == null || conValue == "-")
                                conValue = '';

                            var fn_name = douHelper.getField($_nowTable.instance.settings.fields, fn).title;
                            if (datatype == 'date') {
                                $(this).find('input').val(conValue == '' ? '' : JsonDateStr2Datetime(conValue).DateFormat("yyyy-MM-dd"));
                            }
                            else if (datatype == 'datetime') {
                                $(this).find('input').val(conValue == '' ? '' : JsonDateStr2Datetime(conValue).DateFormat("yyyy-MM-dd HH:mm"));
                            }
                            else {
                                $(this).val(conValue);
                            }
                        });
                    }

                    isDoing = true;
                });
            }
        });
    }

    douoptions.afterUpdateServerData = function (row, callback) {
        jspAlertMsg($("body"), { autoclose: 2000, content: '員工資料更新成功!!', classes: 'modal-sm' },
            function () {
                $('html,body').animate({ scrollTop: $_masterTable.offset().top }, "show");
            });

        //(no callback)更新dou的rowdata
        $_masterTable.instance.updateDatas(row);

        //callback();
    }
   
    douoptions.tableOptions.onCheck = function (row, e) {

        var fno = row.Fno;
        if (aryCheck.indexOf(fno) == -1) {
            aryCheck.push(fno);
        }

        return false;
    };

    douoptions.tableOptions.onUncheck = function (row, e) {

        var fno = row.Fno;
        aryCheck = aryCheck.filter(a => a != fno);

        return false;
    };
    
    douoptions.tableOptions.onCheckAll = function (rows, b) {
        var aryDatas = rows.map(function (data) {
            return data.Fno;
        });

        //全選(merge 不重複)        
        aryCheck = aryCheck.concat(aryDatas.filter(ele => !aryCheck.includes(ele)));
    };

    douoptions.tableOptions.onUncheckAll = function (a, rows) {
        var aryDatas = rows.map(function (data) {
            return data.Fno;
        });

        //全不勾
        aryCheck = aryCheck.filter(item => !aryDatas.includes(item));
    };

    douoptions.tableOptions.onLoadSuccess = function (datas) {

        //選取數量
        var n_sels = 0;
        $('.bootstrap-table #_table tbody').find('.dou-field-Fno').each(function (index) {
            var fno = $(this).text();
            if (aryCheck.indexOf(fno) > -1) {
                n_sels++;

                var a = $(this).closest('tr').find('.bs-checkbox');                
                //用trigger，不使用jquery保持資料正確性
                $('#_table').bootstrapTable('check', index);
            }
        });

        //若清單選取數量都勾,全選也勾
        if (n_sels > 0) {
            if (n_sels == datas.rows.length) {
                //$('input[name="btSelectAll"]').prop('checked', true);
                $('#_table').bootstrapTable('checkAll');
            }
        }

        return false;
    };

    //清單匯出新人簡報
    var a = {};
    a.item = '<span class="btn btn-success glyphicon glyphicon-download-alt"> 新人簡報</span>';
    a.event = 'click .glyphicon-download-alt';
    a.callback = function importQdate(evt) {

        //aryCheck(勾選員編)
        if (aryCheck.length == 0) {
            alert('尚未勾選員工');
            return false;
        }

        helper.misc.showBusyIndicator();
        $.ajax({
            url: app.siteRoot + 'Emp/ExportPPtNew',
            datatype: "json",
            type: "Post",
            data: { "Fnos": aryCheck },
            success: function (data) {
                if (data.result) {
                    location.href = app.siteRoot + data.url;
                    //alert("產出新人簡報成功：");
                } else {
                    alert("產出新人簡報失敗：\n" + data.errorMessage);
                }
            },
            complete: function () {
                helper.misc.hideBusyIndicator();
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                alert(err.Message);
                helper.misc.hideBusyIndicator();
            }
        });
    };

    //清單匯出晉升員工
    var b = {};
    b.item = '<span class="btn btn-success glyphicon glyphicon-download-alt"> 晉升簡報</span>';
    b.event = 'click .glyphicon-download-alt';
    b.callback = function importQdate(evt) {

        //aryCheck(勾選員編)
        if (aryCheck.length == 0) {
            alert('尚未勾選員工');
            return false;
        }

        helper.misc.showBusyIndicator();
        $.ajax({
            url: app.siteRoot + 'Emp/ExportPPtPromote',
            datatype: "json",
            type: "Post",
            data: { "Fnos": aryCheck },
            success: function (data) {
                if (data.result) {
                    location.href = app.siteRoot + data.url;
                    //alert("產出晉升簡報成功：");
                } else {
                    alert("產出晉升簡報失敗：\n" + data.errorMessage);
                }
            },
            complete: function () {
                helper.misc.hideBusyIndicator();
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                alert(err.Message);
                helper.misc.hideBusyIndicator();
            }
        });
    };

    //取消所有勾選
    var c = {};
    c.item = '<span class="btn btn-secondary check-cancel"> 取消所有勾選</span>';
    c.event = 'click .check-cancel';
    c.callback = function importQdate(evt) {
        //alert('aaa11');
        aryCheck = [];
        $('#_table').bootstrapTable('uncheckAll');
        alert('已取消勾選');
    };

    douoptions.useMutiSelect = true;
    douoptions.appendCustomToolbars = [a, b, c];

    var $_masterTable = $("#_table").DouEditableTable(douoptions).on($.dou.events.add, function (e, row) {
        
        //錨點
        var aPoint = $_masterTable.instance.settings.rootParentContainer;
        $('html,body').animate({ scrollTop: $(aPoint).offset().top }, "show");

        //trigger清單(新增row)編輯按鈕的，
        $_masterTable.DouEditableTable("editSpecificData", row);

    }); //初始dou table

    function SetDouEmpDa1(datas) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa1/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '通訊方式';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? [datas] : [{}];
            _opt.datas = datas;

            _opt.singleDataEdit = true;
            _opt.editformWindowStyle = $.editformWindowStyle.showEditformOnly;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.afterCreateEditDataForm = function ($container, row) {
                //保留確定按鈕
                $container.find('.modal-footer button').hide();
                $container.find('.modal-footer').find('.btn-primary').show();

                //加提示字
                var $p1 = $('div[data-field=da01]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">同護照英文姓名</span>';
                $(remind).appendTo($p1);

                var $p3 = $('div[data-field=da06]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">請取整數</span>';
                $(remind).appendTo($p3);

                var $p4 = $('div[data-field=da07]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">請取整數</span>';
                $(remind).appendTo($p4);

                var $p5 = $('div[data-field=da15]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">限填數字</span>';
                $(remind).appendTo($p5);

                var $p6 = $('div[data-field=da24]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">限500字(Ex: 空氣污染防制技術、廢棄物資源化、薄膜水處理技術…)</span>';
                $(remind).appendTo($p6);

                var $p7 = $('div[data-field=ProfilePhoto]').find('label');
                var remind = '<span class="text-danger fw-lighter ms-1">可修改無法刪除</span>';
                $(remind).appendTo($p7);
            }

            _opt.afterUpdateServerData = _opt.afterAddServerData = function (row, callback) {
                jspAlertMsg($("body"), { autoclose: 2000, content: '通訊方式更新成功!!', classes: 'modal-sm' },
                    function () {
                        $('html,body').animate({ scrollTop: $_d1Table.offset().top }, "show");
                    });

                //(no callback)更新dou的rowdata
                $_d1Table.instance.updateDatas(row);

                ////callback();
            }

            //實體Dou js                                
            $_d1Table = $_d1EditDataContainer.douTable(_opt);
        });
    }

    function SetDouEmpDa4(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa4/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '學歷';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            _opt.tableOptions.sortName = 'da404';
            _opt.tableOptions.sortOrder = 'desc';

            _opt.afterCreateEditDataForm = function ($container, row) {
                //加提示字
                var $p1 = $('div[data-field=da404]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">年月(201609)</span>';
                $(remind).appendTo($p1);

                var $p2 = $('div[data-field=da405]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">年月(20206)</span>';
                $(remind).appendTo($p2);
            };

            //實體Dou js
            $_d4Table = $_d4EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa5(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa5/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '經歷';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            _opt.tableOptions.sortName = 'da504';
            _opt.tableOptions.sortOrder = 'desc';

            _opt.afterCreateEditDataForm = function ($container, row) {
                //加提示字
                var $p1 = $('div[data-field=da501]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">若會內職務填「部門名稱」</span>';
                $(remind).appendTo($p1);

                var $p2 = $('div[data-field=da504]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">年月(201609)</span>';
                $(remind).appendTo($p2);

                var $p3 = $('div[data-field=da505]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">年月(20206), 若現職填「迄今」</span>';
                $(remind).appendTo($p3);

                var $p4 = $('div[data-field=da506]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">若會內職務「不需填寫」</span>';
                $(remind).appendTo($p4);

                var $p5 = $('div[data-field=da507]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right"> \
                                (限1000字) 備註：請條列式工作內容，如下所示… </br>\
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1. oooo... </br> \
                              </span> ';
                $(remind).appendTo($p5);
            };

            //實體Dou js
            $_d5Table = $_d5EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa6(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa6/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '家庭狀況';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            _opt.afterCreateEditDataForm = function ($container, row) {
                //加提示字
                var $p1 = $('div[data-field=da603]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">年月日(20160909)</span>';
                $(remind).appendTo($p1);
            }

            //實體Dou js
            $_d6Table = $_d6EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa7(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa7/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '外語檢定';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            _opt.tableOptions.sortName = 'da702';
            _opt.tableOptions.sortOrder = 'desc';

            _opt.afterCreateEditDataForm = function ($container, row) {
                //加提示字
                var $p1 = $('div[data-field=da702]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">年月(201609)</span>';
                $(remind).appendTo($p1);
            }

            //實體Dou js
            $_d7Table = $_d7EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa8(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa8/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '專業資格';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            _opt.tableOptions.sortName = 'da802';
            _opt.tableOptions.sortOrder = 'desc';

            _opt.afterCreateEditDataForm = function ($container, row) {
                //加提示字
                var $p1 = $('div[data-field=da802]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">年月(201609)</span>';
                $(remind).appendTo($p1);
            }

            //實體Dou js
            $_d8Table = $_d8EditDataContainer.douTable(_opt);
        });
    };

    function SetDouEmpDa9(datas, Fno) {
        $.getJSON($.AppConfigOptions.baseurl + 'EmpDa9/GetDataManagerOptionsJson', function (_opt) { //取model option

            _opt.title = '著作';

            //取消自動抓後端資料
            _opt.tableOptions.url = undefined;

            datas = datas ? datas : [{}];
            _opt.datas = datas;

            //////初始options預設值
            ////douHelper.setFieldsDefaultAttribute(_opt.fields);//給預設屬性

            _opt.editformSize = { minWidth: 700 };
            _opt.beforeCreateEditDataForm = function (row, callback) {
                row.Fno = Fno;

                callback();
            };

            _opt.afterCreateEditDataForm = function ($container, row) {
                //加提示字
                var $p1 = $('div[data-field=da901]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">Ex:論文、碩士論文、研討會論文、期刊論文、手冊、叢書...</span>';
                $(remind).appendTo($p1);

                var $p2 = $('div[data-field=da902]').find('label');
                var remind = '<span class="text-danger fw-lighter pull-right">(限1000字)</span>';
                $(remind).appendTo($p2);
            }

            //實體Dou js
            $_d9Table = $_d9EditDataContainer.douTable(_opt);
        });
    };
});