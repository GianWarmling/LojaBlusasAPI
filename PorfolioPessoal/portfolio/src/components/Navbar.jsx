import { Link } from "react-router-dom";

function Navbar() {
    return (
        <div>
            <Link to={"/"}>Home</Link>
            <Link to={"/sobre"}>Sobre</Link>
            <Link to={"/projetos"}>Projetos</Link>
            <Link to={"/contato"}>Contatos</Link>
        </div>
    );
}

export default Navbar;