var dtable;
$(function () {
    doSearch();
    $('#btnSearch').click(doSearch);
    $('#btnExport').click(excelExport);
});

function excelExport() {
    $('#exportLink').attr('href',
        apiaddress +
        '/api/Meal/ExportExcel?nameOrMobile=' + $('#nameOrMobile').val() +
        '&startName=' + $('#sdate').val() +
        '&endName=' + $('#edate').val());
    $('#exportLink').click();
}

function doSearch() {
    if (dtable != null) {
        dtable.fnClearTable(0);
        dtable.fnDraw(); //重新加载数据
    } else {
        layer.load(2);
        dtable = $("#datatable").dataTable({
            "sDom": '<"top">rt<"bottom"flip><"clear">',
            "ordering": false,
            "bAutoWidth": false,
            "bFilter": false, // 搜索栏
            "bServerSide": true,
            "sAjaxSource": apiaddress + '/api/Meal/Meals',
            "fnServerParams": function (aoData) {
                aoData.push({
                    "name": "nameOrMobile",
                    "value": $("#nameOrMobile").val()
                });
                aoData.push({
                    "name": "startName",
                    "value": $("#sdate").val()
                });
                aoData.push({
                    "name": "endName",
                    "value": $("#edate").val()
                });
            },
            "aoColumns": [
                { "mData": "mobile" },
                { "mData": "realname" },
                { "mData": "grade" },
                {
                    "mData": "tradeTime",
                    "orderable": false, // 禁用排序
                    "sDefaultContent": "",
                    "sWidth": "10%",
                    "render": function (data, type, full, meta) {
                        //时间格式化
                        return moment(data).format("YYYY-MM-DD HH:mm:ss");
                    }
                }
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {//相当于对字段格式化

            },
            "fnServerData": function (sSource, aoData, fnCallback) {
                $.ajax({
                    "type": 'get',
                    headers: { 'Authorization': localStorage.getItem('token') },
                    "url": sSource,
                    "contentType": "application/json",
                    "data": aoData,
                    "success": function (resp) {
                        layer.load(2);
                        if (resp.code !== '000000') {
                            localStorage.clear();
                            top.location.href = '/login.html';
                        }
                        fnCallback(resp);
                        $(".cx-table-box").show();
                        layer.closeAll('loading');
                    }
                });
            }
        });
    }
}

function getDateStr(AddDayCount) {
    var dd = new Date();
    dd.setDate(dd.getDate() + AddDayCount);   //获取AddDayCount天后的日期
    var year = dd.getFullYear();
    var mon = dd.getMonth() + 1;              //获取当前月份的日期
    if (mon < 10)
        mon = '0' + mon;
    var day = dd.getDate();
    if (day < 10)
        day = '0' + day;
    return year + "-" + mon + "-" + day;
}
function todayDate(b) {
    $(".cx-date-box").find('span').removeClass('active');
    var self = $(b);
    self.toggleClass('active');


    $("#sdate").val(getDateStr(0));
    $("#edate").val(getDateStr(0));
}
function yesterdayDate(b) {
    $(".cx-date-box").find('span').removeClass('active');
    var self = $(b);
    self.toggleClass('active');
    $("#sdate").val(getDateStr(-1));
    $("#edate").val(getDateStr(-1));

}
function recentDate(b) {
    $(".cx-date-box").find('span').removeClass('active');
    var self = $(b);
    self.toggleClass('active');
    $("#sdate").val(getDateStr(-7));
    $("#edate").val(getDateStr(0));
}
function monthDate(b) {
    $(".cx-date-box").find('span').removeClass('active');
    var self = $(b);
    self.toggleClass('active');

    $("#sdate").val(getDateStr(-30));
    $("#edate").val(getDateStr(0));
}
function resetData() {
    $('.cx-account-list option:first').prop('selected', 'selected');
    $('.cx-canteen option:first').prop('selected', 'selected');
    $('.cx-location option:first').prop('selected', 'selected');
    $("#sdate").val('');
    $("#edate").val('');
}