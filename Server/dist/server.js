"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const body_parser_1 = __importDefault(require("body-parser"));
const socket_io_1 = __importDefault(require("socket.io"));
const app = express_1.default();
const port = 3000;
app.use(body_parser_1.default.json());
app.get("/", (req, res) => {
    res.json({
        message: "Received",
        version: "1.0.0"
    });
});
const server = app.listen(port, "192.168.0.108", (error) => {
    if (error) {
        console.error(error);
    }
    console.log(`Server running from port: ${port}`);
});
const sockets = socket_io_1.default(server);
sockets.on("connection", (socket) => {
    console.log("SOCKET CONNECTED", socket.id);
    socket.on("movement", (data) => {
        sockets.emit("movement_2", data);
    });
});
//# sourceMappingURL=server.js.map