-- local file = io.open ("data.txt")
local file = io.stdin
local added_by_them = {}
local added_by_us = {}
local both_deleted = {}
local both_modified = {}

if file then
    -- process input
    for line in file:lines() do
        -- local _,merge_type,path = unpack(line:split(" "))
        local merge_type,path = line:match("#%s+(.+):%s+(.+)")
        if merge_type and path then
            -- io.write( merge_type .. ": " .. path .. "\n" )
            if merge_type == "added by them" then
                added_by_them[#added_by_them+1] = path
            elseif merge_type == "added by us" then 
                added_by_us[#added_by_us+1] = path
            elseif merge_type == "both deleted" then 
                both_deleted[#both_deleted+1] = path
            elseif merge_type == "both modified" then 
                both_modified[#both_modified+1] = path
            end
        end
    end

    -- process output
    io.write( "echo ---------- added by them ----------\n" )
    for i=1,#added_by_them do io.write( "git checkout --theirs " .. added_by_them[i] .. "\n" ) end

    io.write( "echo ---------- added by us ----------\n" )
    for i=1,#added_by_us do io.write( "git checkout --ours " .. added_by_us[i] .. "\n" ) end

    io.write( "echo ---------- both deleted ----------\n" )
    for i=1,#both_deleted do io.write( "git rm " .. both_deleted[i] .. "\n" ) end

    io.write( "echo ---------- both modified ----------\n" )
    for i=1,#both_modified do io.write( "git checkout --theirs " .. both_modified[i] .. "\n" ) end

    io.write( "git add .\n" )
else
    assert ( false, "failed to open file data.txt" )
end
