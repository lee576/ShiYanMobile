$(() => {
    var verifyCodeImg = $('#verifyCodeImg');
    verifyCodeImg.css("cursor", "pointer");
    createValidateCode();
    verifyCodeImg.click(createValidateCode);
});

function createValidateCode() {
    $.ajax({
        type: "get",
        url: apiaddress + "/api/Login/CreateValidateCode",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.token) {
                var src = "data:image/png;base64," + data.base64Str;
                $('#verifyCodeImg').attr("src", src);
                $.cookie('validateKey', data.token, { expires: 1 });
            }
        }
    });
}

$(".J_Submit").click(function () {
    var obj = {};
    $.each($("#loginForm").serializeArray(), (i, o) => obj[o.name] = o.value);
    if (!obj.UserName) {
        layer.msg("用户名不得为空！", { icon: 2, time: 1000 });
        return;
    }
    if (!obj.Password) {
        layer.msg("密码不得为空！", { icon: 2, time: 1000 });
        return;
    }
    if (!obj.ValidateCode) {
        layer.msg("验证码不得为空！", { icon: 2, time: 1000 });
        return;
    }

    $.ajax({
        url: apiaddress + '/api/Authorize/Token',
        data: JSON.stringify($('#loginForm').serializeJSON()),
        type: "post",
        contentType: 'application/json',
        headers: { 'validateKey': $.cookie('validateKey') },
        success: function (data) {
            if (data.code === "000000") {
                localStorage.setItem('token', "bearer " +  data.token);
                location.href = "index.html";
            } else {
                layer.msg(data.msg, {
                    icon: 2
                }, function () {
                    layer.closeAll('loading');
                });
                createValidateCode();
            }
        },
        error: function (e) {
            layer.msg('登录失败！', {
                icon: 2
            });
        }
    });
});	