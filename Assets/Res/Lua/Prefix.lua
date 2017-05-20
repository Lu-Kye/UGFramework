-- @filepath: Start from LuaRootPath without file extension(.lua)
function include(filepath)
    if CS.UnityEngine.Application.isEditor == true or CS.Macro.DEBUG == true then
        return CS.XLuaManager.Instance:Load(filepath)
    else
        return require(filepath)
    end
end