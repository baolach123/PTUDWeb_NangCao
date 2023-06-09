import React from "react";
import SearchForm from "./SearchForm";
import CategoriesWidget from "./CategoriesWidget";

const Sidebar = ()=>{
    return(
        <div className='pt-4 ps-2'>
                <SearchForm />

                <CategoriesWidget/>
            <h1>
                Bai viet noi bat
            </h1>
            <h1>
                Dang ky nhan tin moi
            </h1>
            <h1>
                Tag cloud
            </h1>
        </div>
    )
}

export default Sidebar;