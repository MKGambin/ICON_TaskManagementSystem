import React, { useEffect, useState } from "react";
import axios from "axios";

const UserList = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        axios
            .get(`/api/users`)
            .then((res) => {
                //alert(JSON.stringify(res));
                setUsers(res.data);
                setLoading(false);
            })
            .catch((err) => {
                //alert(JSON.stringify(err));
                console.error(err);
                setError("Failed to load users.");
                setLoading(false);
            });
    }, []);

    const handleDelete = async (id) => {
        try {
            await axios.delete(`/api/users/${id}`);
            setUsers(users.filter((x) => x.identifier !== id));
        } catch (err) {
            //alert(JSON.stringify(err));
            console.error(err);
            alert("Failed to delete user.");
        }
    };

    if (loading) return <p>Loading users...</p>;
    if (error) return <p>{error}</p>;

    const thTdStyle = { border: "1px solid #FFF", padding: "0.5rem", textAlign: "left", };

    return (
        <div>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1rem" }}>
                <h3>Users</h3>
                <button onClick={() => window.location.href = `/users/create`}>
                    Create
                </button>
            </div>

            <hr />
            <table style={{ width: "100%", borderCollapse: "collapse", }}>
                <thead>
                    <tr>
                        <th>Email</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {users.length === 0 ? (
                        <tr>
                            <td colSpan={2} style={{ textAlign: "center" }}>
                                No users available.
                            </td>
                        </tr>
                    ) : (
                        users.map((user) => (
                            <tr key={user.identifier}>
                                <td style={thTdStyle}>{user.email}</td>
                                <td style={thTdStyle}>
                                    <button onClick={() => window.location.href = `/users/update/${user.identifier}`} style={{ marginRight: "0.5rem" }}>
                                        Edit
                                    </button>
                                    <button onClick={() => window.location.href = `/taskitems/${user.identifier}`} style={{ marginRight: "0.5rem" }}>
                                        Tasks
                                    </button>
                                    <button onClick={() => handleDelete(user.identifier)}>
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default UserList;
